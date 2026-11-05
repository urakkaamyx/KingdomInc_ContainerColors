"""Package the exact installed and isolated-runtime-tested Container Colors DLL."""
from pathlib import Path, PurePosixPath
import hashlib
import json
import re
import tempfile
import xml.etree.ElementTree as ET
import zipfile

PROJECT = Path(__file__).resolve().parents[1]
GAME = PROJECT.parents[1]
OUT = PROJECT / 'Releases'
VERSION = ET.parse(PROJECT / 'ContainerColors.csproj').getroot().findtext('PropertyGroup/Version')


def digest(data):
    return hashlib.sha256(data).hexdigest()


def archive(path, entries):
    with zipfile.ZipFile(path, 'w', compression=zipfile.ZIP_DEFLATED, compresslevel=9) as output:
        for name, data in sorted(entries.items()):
            parts = PurePosixPath(name)
            assert not parts.is_absolute() and '..' not in parts.parts and ':' not in name
            output.writestr(name, data)
    with zipfile.ZipFile(path) as check:
        assert check.testzip() is None and set(check.namelist()) == set(entries)
        with tempfile.TemporaryDirectory(prefix='PackageExtract-', dir=PROJECT / 'Verification') as tmp:
            root = Path(tmp).resolve()
            assert root.parent == (PROJECT / 'Verification').resolve()
            check.extractall(root)
            assert {p.relative_to(root).as_posix(): p.read_bytes() for p in root.rglob('*') if p.is_file()} == entries


def main():
    dll = (PROJECT / 'bin/Release/netstandard2.1/ContainerColors.dll').read_bytes()
    assert dll == (GAME / 'BepInEx/plugins/ContainerColors/ContainerColors.dll').read_bytes()
    reports = [json.loads((PROJECT / 'Verification' / name).read_text()) for name in ('runtime-tests.json', 'compatibility-tests.json')]
    for report in reports:
        assert report['passed'] and not report['error'] and report['count'] >= 70
        assert report['version'] == VERSION and report['dllSha256'] == digest(dll)
        assert report['foundationSha256'] == digest((GAME / 'BepInEx/plugins/Foundation/Foundation.dll').read_bytes())
        assert any('Foundation public registry owns' in check for check in report['checks'])
        assert any('Mods tab displays exactly one' in check for check in report['checks'])
        assert any('Existing unlocked saves retain research' in check for check in report['checks'])
        assert any('Unmodified world renderer' in check for check in report['checks'])
        assert any('No Unity runtime errors' in check for check in report['checks'])
        assert any('Native research purchase spends the exact price' in check for check in report['checks'])
        assert any('Purchased research survives full save reload' in check for check in report['checks'])
        assert any('Full save reload restores label text and exact RGBA' in check for check in report['checks'])
        assert any('Unmodified world renderer loads colored labels' in check for check in report['checks'])
    assert any('Sharable Blueprints capture/parse/import preserves label text' in check for check in reports[1]['checks'])
    for log in ('unity-isolated.log', 'unity-companion.log'):
        text = (PROJECT / 'Verification' / log).read_text()
        assert 'Container Colors diagnostic passed:' in text
        assert 'NullReferenceException' not in text and '[Error  :Container Colors]' not in text
    # Keep user-facing documentation focused on this mod, not QA reports or compatibility catalogs.
    forbidden = r'verification|runtime|diagnostic|isolated|companion|Sharable Blueprints|Party[ -]?(?:Up|Node)|Ledger|Leyline|Path[ -]?Finder|Factory API|Foundation'
    for name in ('README.md', 'Nexus/README.txt', 'Nexus/NEXUS_DESCRIPTION.bbcode.txt'):
        assert not re.search(forbidden, (PROJECT / name).read_text(encoding='utf-8'), re.I), name
    OUT.mkdir(exist_ok=True)
    player = {'BepInEx/plugins/ContainerColors/ContainerColors.dll': dll}
    player.update({'Container Colors ' + name: (PROJECT / 'Nexus' / name).read_bytes() for name in ('README.txt', 'CHANGELOG.txt', 'REQUIREMENTS.txt')})
    player_zip = OUT / f'KingdomInc-ContainerColors-{VERSION}.zip'
    archive(player_zip, player)
    names = ['ContainerColors.csproj', 'Plugin.cs', 'ColorData.cs', 'ColorResearch.cs', 'ColorArtwork.cs', 'ColorRendering.cs', 'PickerGraphics.cs',
             'ContainerPicker.cs', 'RuntimeCheck.cs', 'README.md', '.gitignore', 'install.ps1', 'verify-isolated.ps1',
             'Tools/package_nexus.py', 'Nexus/README.txt', 'Nexus/CHANGELOG.txt', 'Assets/container-colors-icon-v1.png',
             'Assets/custom-container-colors-thumbnail-v2.png', 'Assets/artwork-prompts.txt']
    source_zip = OUT / f'KingdomInc-ContainerColors-{VERSION}-source.zip'
    archive(source_zip, {'_Mods/ContainerColors/' + name: (PROJECT / name).read_bytes() for name in names})
    result = {
        'name': 'Container Colors', 'version': VERSION, 'publication': 'local preparation only',
        'dllSha256': digest(dll), 'gameVersion': reports[0]['gameVersion'], 'unityVersion': reports[0]['unityVersion'],
        'bepInExVersion': reports[0]['bepInExVersion'], 'runtimeChecks': reports[0]['count'], 'companionChecks': reports[1]['count'],
        'researchPrerequisite': 'Stickers', 'researchPriceMultiplier': 1.25, 'researchLocation': 'Tech Tree > Mods',
        'requiredFramework': 'Foundation 0.5.0+', 'foundationSha256': reports[0]['foundationSha256'],
        'testedStickersPrice': reports[0]['stickersPrice'], 'testedContainerColorsPrice': reports[0]['containerColorsPrice'],
        'cleanExtractionPassed': True, 'files': sorted(player),
        'archives': {p.name: {'bytes': p.stat().st_size, 'sha256': digest(p.read_bytes())} for p in (player_zip, source_zip)}
    }
    (OUT / 'RELEASE_VERIFICATION.json').write_text(json.dumps(result, indent=2) + '\n')
    print(json.dumps(result, indent=2))


if __name__ == '__main__':
    main()

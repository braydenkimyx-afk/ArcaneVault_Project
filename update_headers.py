import os
import re

files_updated = 0
missing_header = []

header_cshtml = """@* Name: Brayden Kim *@
@* Student Admin No: 252268E *@
@* Tutorial Group: 07 *@
"""

header_cs = """// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07
"""

for d in ['../ArcaneVault.Web', '../ArcaneVaultAPI']:
    for r, dirs, fs in os.walk(d):
        if 'bin' in r.split(os.sep) or 'obj' in r.split(os.sep) or 'Migrations' in r.split(os.sep):
            continue
        for f in fs:
            if f.endswith('.Designer.cs') or f.endswith('ModelSnapshot.cs'):
                continue
            if not f.endswith('.cs') and not f.endswith('.cshtml'):
                continue
            
            p = os.path.join(r, f)
            with open(p, 'r', encoding='utf-8-sig') as file:
                content = file.read()
            
            new_content = content
            lines = content.split('\n')
            
            is_cshtml = f.endswith('.cshtml')
            
            if is_cshtml:
                # Remove any existing old headers
                new_content = re.sub(r'^\s*@\*\s*Name:.*?\*@\n(\s*@\*\s*Student Admin No:.*?\*@\n\s*@\*\s*Tutorial Group:.*?\*@\n)?', '', new_content, flags=re.MULTILINE|re.IGNORECASE)
                new_content = re.sub(r'^\s*//\s*Name:.*?\n(\s*//\s*Student Admin No:.*?\n\s*//\s*Tutorial Group:.*?\n)?', '', new_content, flags=re.MULTILINE|re.IGNORECASE)
                
                # Check where to insert
                lines = new_content.split('\n')
                if lines and lines[0].strip().startswith('@page'):
                    lines.insert(1, "\n".join(header_cshtml.strip().split("\n")))
                    new_content = '\n'.join(lines)
                else:
                    new_content = header_cshtml + new_content
            else:
                new_content = re.sub(r'^\s*//\s*Name:.*?\n(\s*//\s*Student Admin No:.*?\n\s*//\s*Tutorial Group:.*?\n)?', '', new_content, flags=re.MULTILINE|re.IGNORECASE)
                new_content = header_cs + new_content
            
            if new_content != content:
                with open(p, 'w', encoding='utf-8') as file:
                    file.write(new_content)
                files_updated += 1
            elif not ('Brayden Kim' in content):
                missing_header.append(p)

print('Updated:', files_updated)
print('Missing:', missing_header)

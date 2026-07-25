Save the current Claude Code session as a formatted markdown chat history file.

Saves to `/mnt/c/Users/confksq/Project/AIML-Learn/07_ChatHistory/`.
Pass an optional topic to name the file, e.g. `/share RAG deep dive` →
`Session_ChatHistory_2026-07-19_RAG-deep-dive.md`.

**Health sessions are different** — those APPEND to the thread in `/mnt/c/pers/`.
Do not run this script for those; see the `feedback-save-chat` memory.

Run the following bash command to export the session. Do not modify the script — execute it exactly as written:

```bash
python3 - "$ARGUMENTS" << 'PYEOF'
import json, os, sys, re
from datetime import datetime
from pathlib import Path

# Resolve current project's claude directory
cwd = os.getcwd()
project_key = cwd.replace('/', '-')
claude_dir = Path.home() / '.claude' / 'projects' / project_key

if not claude_dir.exists():
    print(f"ERROR: Claude project directory not found: {claude_dir}")
    sys.exit(1)

# Use CLAUDE_CODE_SESSION_ID to find exactly this terminal's session file
session_id = os.environ.get('CLAUDE_CODE_SESSION_ID')
if session_id:
    candidate = claude_dir / f"{session_id}.jsonl"
    if candidate.exists():
        current_session = candidate
    else:
        jsonl_files = sorted(claude_dir.glob('*.jsonl'), key=lambda f: f.stat().st_mtime, reverse=True)
        if not jsonl_files:
            print("ERROR: No session files found.")
            sys.exit(1)
        current_session = jsonl_files[0]
else:
    jsonl_files = sorted(claude_dir.glob('*.jsonl'), key=lambda f: f.stat().st_mtime, reverse=True)
    if not jsonl_files:
        print("ERROR: No session files found.")
        sys.exit(1)
    current_session = jsonl_files[0]

# Parse messages — keep only user text and assistant text, skip tool calls / thinking
messages = []
with open(current_session, encoding='utf-8') as f:
    for line in f:
        line = line.strip()
        if not line:
            continue
        try:
            obj = json.loads(line)
        except json.JSONDecodeError:
            continue

        msg_type = obj.get('type')
        ts = obj.get('timestamp', '')

        if msg_type == 'user':
            content = obj.get('message', {}).get('content', '')
            # content can be a plain string or a list of blocks
            if isinstance(content, str):
                text = content.strip()
            elif isinstance(content, list):
                parts = []
                for block in content:
                    if isinstance(block, dict) and block.get('type') == 'text':
                        parts.append(block.get('text', '').strip())
                text = '\n'.join(p for p in parts if p)
            else:
                text = ''
            # Skip harness noise: background task notifications are not conversation
            if text and '[SYSTEM NOTIFICATION' not in text and '<task-notification>' not in text:
                messages.append(('user', text, ts))

        elif msg_type == 'assistant':
            content = obj.get('message', {}).get('content', [])
            if not isinstance(content, list):
                continue
            text_parts = []
            for block in content:
                if isinstance(block, dict) and block.get('type') == 'text':
                    t = block.get('text', '').strip()
                    if t:
                        text_parts.append(t)
            text = '\n\n'.join(text_parts)
            if text:
                messages.append(('assistant', text, ts))

if not messages:
    print("No messages found in session.")
    sys.exit(1)

# Determine output path — AIML learning chat histories live here
date_str = datetime.now().strftime('%Y-%m-%d')
save_dir = Path('/mnt/c/Users/confksq/Project/AIML-Learn/07_ChatHistory')

# Create it rather than silently falling back to cwd. A wrong-folder save is
# worse than a failure: it hides the transcript in whatever repo you happened
# to be in. (This is exactly what the old PartsModules default did.)
try:
    save_dir.mkdir(parents=True, exist_ok=True)
except Exception as e:
    print(f"ERROR: cannot create {save_dir}: {e}")
    print("Refusing to guess a location. Fix the path in ~/.claude/commands/share.md")
    sys.exit(1)

# Optional topic slug passed as an argument, e.g. /share RAG-deep-dive
slug = re.sub(r'[^A-Za-z0-9_-]+', '-', ' '.join(sys.argv[1:]).strip()).strip('-')
base_name = f'Session_ChatHistory_{date_str}' + (f'_{slug}' if slug else '')

output_path = save_dir / f'{base_name}.md'
counter = 2
while output_path.exists():
    output_path = save_dir / f'{base_name}_{counter}.md'
    counter += 1

# Build markdown
lines = []
lines.append(f'# Claude Code Session — Chat History')
lines.append(f'**Date:** {date_str}  ')
lines.append(f'**Session ID:** `{current_session.stem}`  ')
lines.append(f'**Messages:** {len(messages)}  ')
lines.append('')
lines.append('---')
lines.append('')

for role, text, ts in messages:
    if role == 'user':
        lines.append('## You')
    else:
        lines.append('## Claude')
    lines.append('')
    lines.append(text)
    lines.append('')
    lines.append('---')
    lines.append('')

with open(output_path, 'w', encoding='utf-8') as f:
    f.write('\n'.join(lines))

print(f"✓ Session saved to: {output_path}")
print(f"  Messages captured: {len(messages)}")
print(f"  Session file: {current_session.name}")
PYEOF
```

After running the script, report back what file was created and how many messages were captured. If there was an error, explain what went wrong.

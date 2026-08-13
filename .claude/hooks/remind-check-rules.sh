#!/bin/bash
cat <<'JSON'
{"hookSpecificOutput": {"hookEventName": "PreToolUse", "permissionDecision": "allow", "additionalContext": "If you don't clearly remember CLAUDE.md and .claude/rules/*.md, re-read them before writing this file."}}
JSON

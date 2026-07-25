---
name: project-mcp-setup
description: "MCP server setup for Claude Code — GitHub via Docker, Azure via CLI"
metadata: 
  node_type: memory
  type: project
  originSessionId: ff68e412-bcb3-4a3f-9601-cca2da48397e
---

GitHub MCP server configured in `~/.claude/settings.json` using Docker image `ghcr.io/github/github-mcp-server`. Token stored as `GITHUB_PERSONAL_ACCESS_TOKEN` env var in the config (OAuth token, may expire — refresh with `gh auth token`).

**Why:** User wanted native GitHub MCP integration comparable to what GitHub Copilot has in VS Code.

**How to apply:** If GitHub MCP tools stop working, first check if the `gho_` token is still valid with `gh auth status`. If expired, run `gh auth login` then update the token in `~/.claude/settings.json`.

Azure MCP server also configured using `npx -y @azure/mcp@latest server start` (package: `@azure/mcp`). Uses existing `az` CLI login — no separate token needed. Subscription ID `a4656eb6-5a57-4548-9e60-0b905e3e16a2`, Tenant ID `e2ba673a-b782-4f44-b0b5-93da90258200` (jmfamily.com).

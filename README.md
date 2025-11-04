# Unstagram (WinForms)

A lightweight Windows Forms tool to analyze your Instagram account export. Load your JSON files, browse normalized lists, open profiles with a double‑click, and see quick insights like:
- They follow me (I don't follow back)
- I follow them (they don't follow back)

Built with .NET 9 and DevExpress WinForms controls.

## Features
- Multi-select JSON/HTML import.
- Auto-maps common Instagram export files to tabs.
- Grids bind to normalized `StringListData` (Href, Value [username], Timestamp).
- Title-based fallback: if JSON lacks `value`, the username is inferred from `title` or `href`.
- Double-click any row to open the profile URL.
- Diff tabs appear when both Followers and Following are loaded:
  - They Follow Me (I Don't)
  - I Follow (They Don't)
- Visual cues: loaded tabs/rows are tinted green; reset returns to neutral/red tint.

## Supported files and JSON roots

Use Instagram's account data export in JSON format. This app recognizes the following filenames and JSON roots:

- followers_1.json
  - Root: array of entries with `string_list_data`.
- following.json
  - Root: `relationships_following`
- blocked_profiles.json
  - Root: `relationships_blocked_users`
- close_friends.json
  - Root: `relationships_close_friends`
- follow_requests_you've_received.json
  - Root: `relationships_follow_requests_received`
- pending_follow_requests.json (aka follow requests sent)
  - Root: `relationships_follow_requests_sent`
- recent_follow_requests.json (aka permanent follow requests)
  - Root: `relationships_permanent_follow_requests`
- recently_unfollowed_profiles.json
  - Root: `relationships_unfollowed_users`
- removed_suggestions.json (aka dismissed suggested users)
  - Root: `relationships_dismissed_suggested_users`
- hide_story_from.json
  - Root: `relationships_hide_stories_from`

Tip: Keep the standard filenames. The app maps files to tabs by filename and then reads the expected root key.

## How to use

1. Start the app.
2. Tools panel > Select content type: choose JSON (default).
3. Click “Open Analyze File(s)” and multi-select any of the supported JSON files.
4. Loaded tabs:
   - Show green tinted rows and a “(Loaded)” suffix in the tab header.
   - Grids are read-only.
5. Interactions:
   - Double-click a row to open the `Href` in your default browser. If `Href` is missing, it builds one from the username.
   - Press Ctrl+C to copy the focused cell’s text.
6. Diff tabs:
   - After loading both Followers and Following, two tabs appear at the beginning:
     - They Follow Me (I Don’t): followers that you don’t follow.
     - I Follow (They Don’t): following that don’t follow you back.

## JSON field expectations

For each profile entry, the app uses:
- `string_list_data`: array of items with
  - `href`: profile link (e.g., https://www.instagram.com/username)
  - `value`: username (optional in some exports)
  - `timestamp`: Unix epoch seconds
- `title`: often the username for some files (e.g., `following.json`, `blocked_profiles.json`)

Normalization rules:
- If `value` is missing, it falls back to `title`. If both are missing but `href` exists, username is extracted from `href`.
- If `href` is missing but username is known, it synthesizes `https://www.instagram.com/{username}/`.

## Build

Prereqs:
- Visual Studio 2022 (17.10+) with .NET 9 SDK.
- DevExpress WinForms components.

Restore and run:
- Open the solution and press F5.

## Publish (for distribution)

Option A — Visual Studio:
- Right-click project > __Publish__.
- __New profile__ > Folder.
- __Show all settings__:
  - Target framework: net9.0-windows
  - Target runtime: win-x64
  - Deployment mode: Self-contained (recommended for users without .NET)
  - File options: enable Single file
- Publish. Zip the publish folder and share.

Option B — CLI:

```bash
dotnet publish src/Unstagram.WinFormApp/Unstagram.WinFormApp.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishTrimmed=false -o ./artifacts/win-x64
```

## Release via GitHub Actions (optional)

Place a workflow at `.github/workflows/publish-winforms.yml` to build on tag push and attach artifacts to a Release. See repository Actions for details.

## Troubleshooting

- Tabs don’t turn red after reloading:
  - Use the Open dialog again. The app resets tabs and colors before loading new files.
- Diff tabs don’t update:
  - Ensure both Followers and Following are loaded in the same session.
- A grid is empty:
  - Confirm the filename and root key match the list above, and the JSON is valid.
- HTML content:
  - Switch content type to HTML before opening to preview HTML files.

## Privacy

All processing happens locally. No data is uploaded.

## License

MIT (unless specified otherwise).


# OLD DOCUMENTATION FOR CONSOLE APP - PLEASE CHECK WINFORMS VERSION ABOVE

# **Ustagram - The Follower/Following Checker**

## **Download Instagram Data**

```
Download your followers_and_following data from instagram.
```
![downloadInstagram](https://github.com/unseensenpai/Unstagram/assets/39537512/db90f745-4e6a-42af-a86d-e70f0a68f5e8)

## **Copy Files To Program Path**

```
Copy "followers_1.json" and "following.json" to root folder.
```
![copyFollow](https://github.com/unseensenpai/Unstagram/assets/39537512/da25067b-00ef-4972-909d-4d767eacd940)


## **Run Program**

```
Run Ustagram.exe in root folder.
```
![runApp](https://github.com/unseensenpai/Unstagram/assets/39537512/4da3921d-2c8e-444c-abbd-93ecb3b9c6dc)

## **Follow Steps**
```
Press 'Enter' key and wait for process.
```
![followSteps](https://github.com/unseensenpai/Unstagram/assets/39537512/a3236f33-a318-46a1-8545-b0bfedc368ce)

## **Done**

```
Press any button to exit.
```

## **Check Results Folder**
```
Go Root/Results folder to check results.
```
![checkFiles](https://github.com/unseensenpai/Unstagram/assets/39537512/7ff7813c-e6d0-4369-863e-3b364019193f)

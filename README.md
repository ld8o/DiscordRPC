# DiscordRPC
**DiscordRPC** is a simple C# library that lets you integrate [Discord Rich Presence](https://discord.com/developers/docs/rich-presence/how-to) into your application using Discord's IPC system.

# Installation
1. Clone the repo or copy the files `DiscordRPC.cs` and `DiscordPipeClient.cs` into your project.
2. or
3. Download the DLL file from the [Releases](https://github.com/ld8o/DiscordRPC/releases) section.
4. In your project:
   - **Right-click on References > Add Reference...**
   - Browse to `DiscordRPC.dll`
   - Click **OK**

# Usage
```csharp
var rpc = new DiscordRichPresence.DiscordRPC();
bool connected = await rpc.ConnectAsync("121"); // Your Discord Application ID

if (connected)
{
    rpc.SetPresence("example", // Details 
  "example", // State
   "example", // Image, Must match an asset from your Discord app
   "example" // Asset Text
); 
}
```

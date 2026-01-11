# Tested on:
V1.614

# Preparation:
You will need to install BepInEx first. The wiki has a guide for this:

https://planet-crafter.fandom.com/wiki/Modding#Installation

When installing my mods, unzip the mod into the BepInEx\Plugins directory, including the folder inside the zip file.

You'll have a directory structure like this:

BepInEx\Plugins\Tjatja - Save Sharing\SaveSharing.dll

Such organization avoids overwriting each others' files if they happen to be named the same as well as allows removing plugin files together by deleting the directory itself.

⚠️ Enabling mods became a bit more involved in 0.7 and beyond.

The new Unity version the game uses has a feature/bug that prevents all mods from running beyond their initialization phase. To work around it, find the BepInEx\config\BepInEx.cfg file, and in it, set

HideManagerGameObject = true

# How to setup:

After installing BepInEx and unzipping the file into your folder:
- run the game.
- exit the game.
- Find BepInEx\config\Tjatja.theplanetcraftermods.savesharing.cfg
- make sure that Enabled = true
- set IPForConnection to the IP or URL of your server host.
- rename your save file of choice to Server-1.txt.
- upload "upload.php" and "Server-1.txt" to the server.
- Check if you can reach http://IPForConnection/upload.php after doing so.
- When connecting to the page it should result in an upload error.
- Run the game. 

# When inviting friends:

- Invite them like you normally would through Steam/GoG.
- If they ever want to host the game, Make sure they installed the saveSharing.dll into their BepInEx, and they set the IPForConnection.
- When the host leaves the game, so will the rest of the players, make sure to agree who will host next and re-invite like normally.

- Make sure to check if nobody is already hosting the game before starting the save file as host to prevent conflicts, if you accidentally  did host it while someone was already doing so, alt+f4 to kill the game, or simply kill your BepInEx window, and ask for an invite instead.

# When setting up a server:

- do make sure to install the PHP service from Apache if you end up using Wamp or Xampp.

  Windows based tutorial:
  https://www.geeksforgeeks.org/websites-apps/how-to-use-apache-webserver-to-host-a-website/

  Localhost tutorial:
  https://www.wikihow.com/Install-and-Configure-Apache-Webserver-to-Host-a-Website-from-Your-Computer

  Ubuntu based tutorial:
  https://www.digitalocean.com/community/tutorials/how-to-configure-the-apache-web-server-on-an-ubuntu-or-debian-vps

Special Thanks to
@Akarnokd for the great code examples,
@Nicki0 for helping me test and giving advice.
The Planet Crafter wiki contributors  for the great guides on modding the game.

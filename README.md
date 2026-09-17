# Beat It 
## What it does:
Your music is the level. Import any Mp3 file into the game's music folder and speed through the highway along the beats of the song.
Dodge the traffic and get the beatBlocks to maintain a streak!!
## How to play:
- Download the Zip file from [itch.io](https://ksfdev.itch.io/beat-it)
- Extract it and its ready to run.
- Click play and it will take you to your song library.
- If its empty then click on the "Song Folder" button and it will redirect you to the folder you need your songs to be in.
- paste your mp3 files in that folder and restart the game for it to refresh.
- you should see the list of songs in that folder.
- Click any one to Play on it!
## Controls:
Very Simple A/D or Left Arrow/Right Arrow controls to weave the bike left and right to dodge traffic.
If you do hit a car the screen turns slightly red , hit more cars in a row and boom you got beated on!(you lost)
## How this was made:
Started when i was playing the game "dancing line" on my phone and its a really cozy and good game but too cozy for my liking at times, i needed something fast paced high energy , couldnt really find anything like that so i decided to just ...make it!
This was my first time dealing with audio analysis so I had to learn a lot of stuff, i was going to go with a Fast Fourier Transform to dissect the audio but then i was told that it was overkill for this game, and a local energy based beat detection system would suffice.
How that works is that The audio is broken down into a LARGE array of floats and the floats are simply the gain of the audio so if its 0 then that means there is no sound in the audio and 1 means max amplitude, so local beat detection was that when in a range of array entries that i specified  myself, there was a sudden spike in the value of the float and it was above the set threshold that timestamp was marked as a beat. 
Then on the beat a beat block was placed that gave it a satisfying feel of collecting. 
But this only took care of the Z position of the beat block if nothing was done then all the blocks will be in a straight line, so then the second analysis came from the frequency of the major sound in the audio, i made 6 separate bands of frequency declaring each of them as a "level" and each level was assigned a part of the road from left to right, so when the pitch of the audio dropped the beatblocks spawned towards the left and when it increased they spawned towards the right, so this gave the game a sense of skill to it idk? coz now its not completely random it asks the player to listen to the song and deduce whether further blocks will be to the left or to the right before they are even seen. 
## Tech Used:
Unity, C# , Low Poly car assets my friend made.
## Conclusion:
The game will have bugs and is not very polished since my exams started so i had to stall it, but i will try to finish it afterwards, this can be seen as the MVP.
If you played this game, Thank you please give feedback on what i can improve.

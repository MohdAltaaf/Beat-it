# BEAT IT

## Your music is the level.

A synthwave motorcycle runner where the road, the collectibles, and the traffic are all generated from a song you actually own. Drop in any MP3, and the game listens to it — beat by beat — to build a highway that only exists because that specific track does.



## What it actually does
Import any MP3. The game reads it, finds the beats, and figures out roughly where the melody rises and falls.
A road gets built to match: collectible beat blocks land exactly on the drum hits, and the path between them curves in the direction the melody moves.
Traffic weaves through the level too — some of it sitting directly on your line, forcing an actual choice between chasing a streak and swerving to survive.
Get clipped by a car and the screen reddens instead of ending the run outright. Get clipped enough times in a row without recovering, and it does.
Survive the whole song, and the camera holds still while the bike rides off toward the horizon on its own.

No two playthroughs look the same, because no two songs are the same.

## How to play

Controls: A/D or Left/Right to weave across the road.

Your music: On first launch, hit Play, then "Open Songs Folder" on the song select screen. Drop your MP3s in, back out to the main menu, hit Play again to refresh the list.

## The story behind it

This started as a design doc for something I genuinely didn't know how to build. I'd never touched audio analysis before — I knew Unity, I knew C#, and I had a pretty clear picture of the feeling I wanted (weaving through a highway that's actually reacting to the song you picked), but the actual how of "read an MP3 and find the beat" was a total blank.

So I built it the only way that made sense: one small, testable piece at a time. Load the file. Confirm you can actually read the raw samples back out — and immediately learn the hard way that array indices and seconds-into-the-song are not the same scale when your audio is stereo and interleaved. Get a single number (energy in a window) that's louder on a kick drum than on silence. Watch it also light up on vocals and melody, because raw loudness doesn't know the difference between a kick and a chorus — and go learn what a low-pass filter actually does about that. Watch beats land audibly late, and find out that's because you're timestamping the start of a window instead of hunting for the actual peak inside it. Rebuild the whole detector around change in energy instead of raw energy, because a sustained bassline was getting picked up as a wall of false beats a flat threshold could never tell apart from a real hit.

None of that was in a tutorial I followed — it was "why is this wrong" one bug at a time, until it wasn't.

The design itself changed shape more than once, too. The original plan kept the bike frozen in place with the whole world scrolling past it, specifically to dodge floating-point drift over a long run. Then a simple particle trail on the bike broke that illusion completely — a trail can't read as "moving forward" if the thing it's attached to never actually moves — so the bike started really driving, and the "drift" problem I'd been defending against turned out to not even apply once I did the math on how far a single song could actually carry it. A whole planned system (recycling road tiles forever) got deleted the moment I realized every level is scoped to one finite, fully-known-in-advance song — there's no "forever" to build for.

The traffic system is maybe my favorite piece of this, honestly. Cars aren't just scattered around — they're placed by solving backwards: given exactly where and when I want a near-miss to happen, and how fast the car should be going, what starting position makes that car arrive there right on cue? Same trick the beat blocks use for their own placement, just pointed at a different problem.

I had a weekend. Saturday noon to Monday, start to finish, for the whole thing — path system, traffic, UI, scoring, fail states, and polish. It shipped.

## Tech
Unity, C# (new Input System)
URP, with a Volume-driven vignette for the hit-feedback effect
No third-party DSP libraries — the beat and melody detection are hand-rolled: single-pole low-pass/band-pass filtering, spectral-flux-style onset detection, and a lightweight spectral-centroid approximation for melody direction, all built directly on raw AudioClip sample data

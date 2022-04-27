# WizAR

A Multiplayer AR game with Speech-To-Text controls. You can play it from two different locations. 
The game is basically a duel between two wizards/users, who can use three spells:

- Attack spell (STT-command: "Fireball") casts a fireball
- Shield spell (STT-command: "Shield") casts an ice wall in front of you
- Attack/Shatter spell (STT-command: "Shatter") casts a spell which breaks the shield of the opponent


https://user-images.githubusercontent.com/38131809/165566639-cca86c43-0c65-425a-ada3-bc130d3286d0.mp4


## Technical background
- For Multiplayer [Mirror Networking](https://mirror-networking.gitbook.io/docs/) is used. It is implemented as a Peer-to-Peer network. So one of the clients acts as the host/server.
- For Speech-To-Text [Android speech Recognizer Plugin](https://assetstore.unity.com/packages/tools/input-management/android-speech-recognizer-plugin-47520/reviews) is used, which uses the default Android STT in the background.

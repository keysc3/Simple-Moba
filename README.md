# Simple-Moba

Framework for player combat and the systems needed for it in a moba type game for learning purposes.

## Features

- Player movement and controls.
- Unit Systems.
    - Stats.
    - Levels.
    - Inventory.
    - Effects.
- Combat
    - Four spell characters.
    - Basic attack.
    - Mana and health usage.
    - Death and respawn.
- User Interface
    - Spell cooldowns and levels.
    - Player stats, level, and experience.
    - Kills, deaths, assists, and creep score.
    - Unit health and mana bars.

## Project Notes

The project is made as a single player game and has no multiplayer components or functionality at the moment but could be refactored for more learning in the future. 
It implements an active champion singleton for testing purposes because of this.

This project started with creating Ahri's abilities from League of Legends but quickly turned into an attempt to build a full combat system. The effects of Ahri's charm caused the pivot because I didn't want to just move the 
game object towards the caster. For scalability purposes I wanted to be able to disable their ability to move and cast spells if they were a player. That started the building of the effects system and caused the decision to 
create a scalable player and combat system, which were also necessary for Ahri's passive.

## Role based authorization using cookies

Simple, mock api showcasing implementation of claim based authorization with **login**, **salt** and **password hash** held in the database.

Role           | Query result
:--------------------:|:--------------------:
<img src="https://user-images.githubusercontent.com/18511595/141474421-b172a266-1a70-4b34-bc07-d02c05d51264.png" style="display: inline-block" width="500">|<img src="https://user-images.githubusercontent.com/18511595/141475083-481f771b-a0f5-4408-8fe3-b6f94eeb530a.png" width="500">
<img src="https://user-images.githubusercontent.com/18511595/141475778-e37b6c84-6358-40e6-a5c7-32d2eb8b6e89.png" width="500">|<img src="https://user-images.githubusercontent.com/18511595/141476917-34b22599-ef04-4849-8276-cd3877a5927f.png" width="500">


#### Request/Response-Domain-DTO layer separation
Api is built with proper separtion. That allows making changes to either domain or database, without the need to change requests or responses which effectively are a form of contract.

#### Password and Hashing
Passwords are not stored in database, password hashes and salts are.
Hashes are generated with 128bit random salt using PBKDF2 with HMACSHA256 key and 10K iterations.
<img src="https://user-images.githubusercontent.com/18511595/141479339-7251b5da-3993-473e-9711-55ac105e37b3.png" width="700">






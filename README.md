# Hinge It! - The Microsoft Surface Duo game
> This [Xamarin.Forms](https://dotnet.microsoft.com/apps/xamarin) Android project is dedicated to the gamification of the [Microsoft Surface Duo](https://www.microsoft.com/en-us/surface/devices/surface-duo)'s hinge. Be the fastest to hinge / fold the phone into the correct angle.

## Gameplay
The game randomizes an angle value. After the player taps on "Start", the player has to hinge / fold the Microsoft Surface Duo as fast as possible into the target angle.

## Game states

**Mocked**
![Summary](docs/summary.png)

## Open questions

- Is the game playable on an actual device?
- Check for min and max angles of real devices
- Should be the device in an 180° position?
- Is the angle treshold of 5 degrees a good choice?
- Does angles of 0 and 360 count as not spanned?

## Caution
Due to the early days of the Surface Duo emulator, the app is missing the hinge angle value reading. You cannot fold your Desktop - hopefully. I try to update this project with new features until it is actual playable.

## Authors
Just me, [Tobi]([https://tscholze.github.io).

## Links
- [Microsoft Docs](https://docs.microsoft.com/en-us/dual-screen/android/get-duo-sdk?tabs=java) for dual screen devices
- [Xamarin](https://dotnet.microsoft.com/apps/xamarin) Homepage

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
Dependencies or assets maybe licensed differently.

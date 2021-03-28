# UnSave

UnSave is an extensible (de)serializer for Unreal Engine 4 save files.

In short, this library can be used to read in unencrypted UE4 save files, parse the data/properties included, and (optionally) write that save data back out to a `.sav` file.

Note that this library is intended for usage in libraries/apps and does not include a user-friendly format. For that, use the excellent [gvas-converter](https://github.com/RagingLightning/gvas-converter).

This library is in an _extreme alpha_ state and probably shouldn't be relied on in it's current state until I've tested and refactored it a bit more.

### Credits

Huge thanks to [@RagingLightning](https://github.com/RagingLightning) and Ilya/[@13xforever](https://github.com/13xforever) for their work on `gvas-converter`. That tool is amazing and a huge design inspiration (as well as filling in a few gaps in my knowledge) for building this.

> Honestly, for most use cases, `gvas-converter` is the better choice: UnSave is only intended for scenarios where extensibility and customization are crucial.

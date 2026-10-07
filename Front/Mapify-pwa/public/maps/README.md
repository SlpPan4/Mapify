# Map layout images

Drop flat, top-down map layout images here to give the strategy constructor a
real backdrop instead of the placeholder grid.

- File name: `<mapId>.png` (or `<mapId>.svg` as a fallback). The map id is the
  numeric id from the backend — see it in the URL query when you pick a map on
  the home page (`/?map=7`), or via `GET /api/maps`.
- Recommended: roughly 4:3 aspect ratio (the canvas keeps the image fully
  visible with `object-fit: contain`), around 1200x900 px for PNG.
- Missing files are fine: the constructor falls back to the flat grid
  placeholder, so layouts can be added map by map.

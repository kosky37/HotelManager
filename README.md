# hotel-manager
### To run:
`.\HotelManager.exe --hotels hotels.json --bookings bookings.json`

`--hotels` and `--bookings` parameters should point to the json files. The path is relative to the .exe file


### Example commands:

`Availability(H1, 20260910-20260911, SGL)`

`Search(H1, 1000, SGL)`

#### bookings.json
```
[
  {
    "hotelId": "H1",
    "arrival": "20240901",
    "departure": "20240903",
    "roomType": "DBL",
    "roomRate": "Prepaid"
  },
  {
    "hotelId": "H1",
    "arrival": "20240902",
    "departure": "20240905",
    "roomType": "SGL",
    "roomRate": "Standard"
  },
  {
    "hotelId": "H1",
    "arrival": "20230902",
    "departure": "20260905",
    "roomType": "SGL",
    "roomRate": "Standard"
  }
]
```

#### hotels.json
```
[
  {
    "id": "H1",
    "name": "Hotel California",
    "roomTypes": [
      {
        "code": "SGL",
        "description": "Single Room",
        "amenities": ["WiFi", "TV"],
        "features": ["Non-smoking"]
      },
      {
        "code": "DBL",
        "description": "Double Room",
        "amenities": ["WiFi", "TV", "Minibar"],
        "features": ["Non-smoking", "Sea View"]
      }
    ],
    "rooms": [
      {
        "roomType": "SGL",
        "roomId": "101"
      },
      {
        "roomType": "SGL",
        "roomId": "102"
      },
      {
        "roomType": "DBL",
        "roomId": "201"
      },
      {
        "roomType": "DBL",
        "roomId": "202"
      }
    ]
  }
]

```
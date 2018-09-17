# Groton Trail Tracker Mobile App

The Groton Trail Tracker is an Eagle Scout project to create a mobile application
and trail data to encourage users to explore the many trails in Groton Massachusetts.

This is the repository for the C# source code for this Xamarin-based mobile
application. It currently supports the two main mobile platforms:

- Android
- iOS

### Application

This app was designed to be used on the Groton Trail Network of Groton, Massachusetts.
Several popular trails have been mapped out with waypoints and images. The application
uses geofencing to check when the user is near a waypoint. Each visited waypoint is
saved to track the users progress in hiking the trails. In addition, a compass is available
to guide the user to the waypoints.

### Development

The solution is done using the Visual Studio Community Edition 2017. You should be
able to download and install this and then open the solution. You will have to install
the Android development tools and use the Android emulators for debugging. The design
is straight-forward MVVM with singleton services.

The UI is in Xamarin Forms to simplify the development, the common logic uses the
old Portable Class Library (PCL) including common embedded resources for data and
images.

To build for iOS, you'll need a Mac with the Visual Studio installed so you can build
the solution and test using the Apple simulators or provision a phone for debugging.

### Trail Data

All trail data is located in the DefaultTrailDef.json file. It references as embedded
resources the image files. Simply add/modify the waypoint data and build the application.

```
[
  {
    "Identifier": "TownForest",
    "Name": "Town Forest",
    "Description": "The Groton Memorial Town Forest is one of Groton's largest trail systems with over 16 miles in trails. This waypoint set will take you along the Loop Trail.",
    "ImageName": "GTNTracker.Images.TownForest.tf8a.jpg",
    "Regions": [
      {
        "Identifier": "TF1",
        "TrailIdentifier": "TownForest",
        "Name": "Trail Start",
        "Description": "Information kiosk at trail start.",
        "ImageName": "GTNTracker.Images.TownForest.tf1a.jpg",
        "IsImageNameURI": false,
        "Center": {
          "Latitude": 42.59727259636167,
          "Longitude": -71.605062726951772
        }
      }
     ]
   }
]
```
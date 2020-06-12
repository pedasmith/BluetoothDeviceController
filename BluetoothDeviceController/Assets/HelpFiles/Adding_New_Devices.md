# How to add a new device to Bluetooth Controller

# Part One: absolute minimum

The absolute minimum work is to make a customized JSON file; this will get up to GitHub and serves as a sort of beacon so that the services and characteristics can be found by other people. The JSON helps to document the actual Bluetooth device. 

## Characteristics Data JSON file

Start by grabbing a snapshot of the device characteristics JSON file. 

Add a new file into **Assets/CharacteristicsData**.
The file must be marked as content/copy if newer

Update the JSON as needed. Very commonly these need to be set
1. ClassName
2. Aliases
3. Description
4. Links
5. Names for the services and characteristics
6. The type of each characteristic data (as opposed to just BYTES|HEX)

# Part Two: specialty pages

It's great when the program can serve up a reasonable page when the device is seen.

## Protocol file

Generate a protocol file and put it into **BluetoothProtocols**.

Check the file and make sure it looks OK

This protocol file will be used by the Page file

## Page file (XAML and .cs)

Generate the Page.xaml and Page.xaml.cs file.

1. Ask Visual Studio to generate an Elegoo_MiniCarPage.xaml "Page" file in the SpecialityPages directory.
2. **NOte**: the SpecialtyPages directory is only for automatically created pages. If you customize a page, it will have to be in the SpecialtyPagesCustom directory
3. Replace the contents of the Page.Xaml and Page.Xaml.Cs files with generated ones

## Add the page into the list in MainPage.cs

There's a list of speciality pages in MainPage.cs; it's how the code decides if a Bluetooth device should be shown/not shown when just devices with specializations should be displayed.

```
            new Specialization (typeof(SpecialtyPages.Elegoo_MiniCarPage), new string[] { "ELEGOO BT16" }, CAR, "Elegoo Mini-Car", "Elegoo small robot car"),

```

Feel free to make new icon types (the **CAR**) in the example; they are just there so the end user can easily tell one type of device from another.

# Part Three: documentation

Everything is better with documentation!

## Device Pictures

For each device (Elegoo_MiniCar) you will need two device pictures: Elegoo_MiniCar-175.png and Elegoo_MiniCar-350.png. These are square PNG files, sizes 175x175 and 350x350, in PNG format.

If at all possible, the device should be centered in the picture and the outside edges should be transparent.

Add them to the project, in Assets/DevicePictures. Mark the pictures as content, copy if newer in Visual Studio.

Be sure that they get added to GitHub.

## Screenshot

Create a screenshot of the new device and the controls. The screnshot should include the entire app screen, including the title bars.

## Device Help

Create an .MD file file in the HelpFiles directory; it will be called e.g. Device_Elegoo_MiniCar.md. Use one of the existing .MD files as a sample.

Be sure to update the screenshot images!

## Add references

The device should have references added as needed:

1. the Welcome.md file 
2. the Help.md file


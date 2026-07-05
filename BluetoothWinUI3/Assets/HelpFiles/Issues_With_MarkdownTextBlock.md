# Issues with the WinUI3 compatible MarkdownTextBlock

## ms-appx: images will pop up a dialog even when IImageProvider has already handled it



## Image height and width

The markdown control seems to have some concept of an image's height and width, which is super handy. The control height and width seems to be handled by adding a suffix, "=*width*x*height*" value. This is similar to how Reddit handles height and width.

However, there are multipe issues:

* If a project's MarkdownTextBlock wrapper code uses the IImageProvider GetImage(), the height and width have been stripped off from the URL that is passed in. 
* The height and width are not passed in as parameters to the IImageProvider GetImage() method.
* The image returned from a GetImage does not get updated with the discovered height and width. That is, when an image is handled natively by the MarkdownTextBlock control (and not by IImageProvider), it will be updated with a height and width. But an image returned from my GetImage is not updated.

In my project, an image width and height can be (optionally) set by adding &size_*600*x*30* (which sets the image to 600 pixels wide and 30 pixels high). My control is required to use a setting which is deliberately incompatible with the rest of the world.

Links:
* [Code for stripping height and width](https://github.com/CommunityToolkit/Labs-Windows/blob/f437e525e57d33c56315ae68577d12d80d21dfb0/components/MarkdownTextBlock/src/Extensions.cs#L410)
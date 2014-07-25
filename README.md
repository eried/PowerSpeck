PowerSpeck
==========

Simplistic presentation software, with a standalone presentation player that runs in Mono ([Raspberry Pi](http://www.raspberrypi.org/), [Beaglebone Black](http://beagleboard.org/Products/BeagleBone+Black), and others). It includes a software to convert from and to PowerPoint (2003 to 2013 versions) using MS PowerPoint interoperation (converter uses MS Office PowerPoint 2010 or newer installed in the machine). Player or Library does not require MS PowerPoint.


Player
---------
<img src=http://content.screencast.com/users/erwinried/folders/Snagit/media/8a13032f-d33e-4e05-8e73-400871b79752/07.23.2014-19.45.png /><br>
Plays a PowerSpeck presentation, including transitions. Can run fullscreen or windowed (_-window_ argument). Run with _-debug_ argument to get additional on-screen information about the presentation.

Converter
---------
<img src=http://content.screencast.com/users/erwinried/folders/Snagit/media/40cd10f4-b42b-4272-8e43-8cd072b23356/07.23.2014-19.42.png /><br>
Utility to convert a PowerPoint presentation into a PowerSpeck one, or viceversa. Requires Office 2010 or newer.

Library
--------
Library to use PowerSpeck presentations in your application.

__Example:__
Small example showing how to open and iterate thru slide elements in a PowerSpeck presentation:

    using PowerSpeckLib;

    class Program
    {
        static void Main(string[] args)
        {
            var presentationFile = "my_presentation.cfg";

            // Loading the slides (with black as default background if not defined)
            foreach (var slide in SlideParser.Parse(presentationFile, Color.Black).Slides)
            {
                // Some properties from this slide
                Console.WriteLine("Slide duration: " + slide.Hold);
                Console.WriteLine("Transition: " + slide.Transition.Type);

                foreach (var item in slide.SlideObjects)
                {
                    switch (item.Type)
                    {
                        case SlideObjectType.Generic:
                            break;
                        case SlideObjectType.Text:
                            break;
                        case SlideObjectType.Image:
                            // Do something here
                            Console.WriteLine("Image X: " + (item as SlideImage).Left);
                            break;
                    }
                }
            }
        }
    }

Demos
==========
Video with PowerSpeck presentation displayed on a LED RGB matrix screen, using a Raspberry Pi:
[![Running on a LED RGB matrix screen via a Raspberry Pi](http://img.youtube.com/vi/IFDBD_Ty9lo/0.jpg)](http://www.youtube.com/watch?v=IFDBD_Ty9lo)

Resources
==========
Source: https://github.com/eried/PowerSpeck <br>
Download last version: http://cl.ly/1f2Z0m111E2i
     

Appendix
==========

PowerSpeck configuration file
------------

    [general]
    ;Use empty.cur to hide the cursor
    cursor=empty.cur
    
    ;Default background color
    background=black
    
    ;Forced update every few ms
    interval=1000
    
    ;Presentation file to load
    load=presentation.cfg
    
    ;Fixed drawing region
    ;top=0
    ;left=0
    ;width=300
    ;height=200

__Notes:__ Color can be defined as <code>#RRGGBB</code> html or [KnownColor](http://msdn.microsoft.com/en-us/library/system.drawing.knowncolor(v=vs.110).aspx). _Interval_ is equivalent to minimum 'tick', use a lower value for more consistent timing. The _fixed region_ specifies an area to draw the transitions (in case you need to draw only an area, for example for a dedicated device)

PowerSpeck presentation file
------------

    [slide1]
    ;Delay in seconds
    hold=500
    transition=CoverRight,500
    ;Add text in posx,posy,size,color,text
    txt1=0,3,20,white,Open
    ;Change default background for this slide
    img1=0,0,72,40,images/boximage.png
    background=red
    
    [slide2]
    ;Delay in seconds
    hold=500
    transition=PanRight,500
    ;Add text in posx,posy,size,color,text
    txt1=2,6,18,white,Every
    ;Change default background for this slide
    img1=0,0,72,40,images/boximage.png
    background=green
    
__Notes:__ Color for the background can be defined as <code>#RRGGBB</code> html or [KnownColor](http://msdn.microsoft.com/en-us/library/system.drawing.knowncolor(v=vs.110).aspx). _Hold_ is the delay per slide, in milliseconds. The transition parameter consist on one of the [Transitions](http://msdn.microsoft.com/en-us/library/microsoft.office.interop.powerpoint.ppentryeffect(v=office.14).aspx) with or without the PowerPoint prefix, followed by the time of that transition, in milliseconds. The currently available Transitions can be seen in this [Enumeration](https://github.com/eried/PowerSpeck/blob/master/PowerSpeckLib/SlideTransitionEffect.cs).
    
Raspberry Pi presentation kiosk
------------
Steps to configure the Raspberry Pi to run as a kiosk with the presentation

__Software setup__

    sudo apt-get update
    sudo apt-get upgrade
    sudo apt-get install mono-complete xutils
  
__To avoid the screen turning off__

    sudo nano /etc/X11/xinit/xinitrc
  
And now in the editor, append:

    setterm -blank 0 -powersave off -powerdown 0
    xset s off
    xset -dpms s off
    mono /path/to/your/assembly.exe
  
__Auto start as kiosk__

    sudo nano /etc/rc.local
  
And now in the editor, replace contents with:

    /bin/bash startx &
    exit 0
  
Make sure that in <code>sudo raspi-config</code> start mode is configured as _console_

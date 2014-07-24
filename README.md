PowerSpeck
==========

Simplistic presentation software, with a player that runs in Mono (Raspberry Pi, Beaglebone Black, etc)


Player
---------
<img src=http://content.screencast.com/users/erwinried/folders/Snagit/media/8a13032f-d33e-4e05-8e73-400871b79752/07.23.2014-19.45.png /><br>
Plays a PowerSpeck presentation, including transitions. Can run fullscreen or windowed. 

Converter
---------
<img src=http://content.screencast.com/users/erwinried/folders/Snagit/media/40cd10f4-b42b-4272-8e43-8cd072b23356/07.23.2014-19.42.png /><br>
Utility to convert a PowerPoint presentation into a PowerSpeck one, or viceversa.

Library
--------
Library to use PowerSpeck presentations on your application.

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

Appendix
==========
    
Steps to configure the Raspberry Pi to run as a kiosk with the presentation
------------

__Software setup__

  sudo apt-get update
  sudo apt-get upgrade
  sudo apt-get install mono-completexutils
  
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
  
Make sure that in ´sudo raspi-config´ start mode is configured as "console"

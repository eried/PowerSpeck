namespace PowerSpeckLib
{
    public enum SlideTransitionEffect
    {
        None = -1, 
        /*Mixed, // Mixed
        Cut, // Cut
        CutThroughBlack, // Cut Through Black
        Random, // Random
        BlindsHorizontal, // Blinds Horizontal
        BlindsVertical, // Blinds Vertical
        CheckerboardAcross, // Checkerboard Across
        CheckerboardDown, // Checkerboard Down*/
        CoverLeft, // Cover Left
        CoverUp, // Cover Up
        CoverRight, // Cover Right
        CoverDown, // Cover Down
        CoverLeftUp, // Cover Left Up
        CoverRightUp, // Cover Right Up
        CoverLeftDown, // Cover Left Down
        CoverRightDown, // Cover Right Down
        /*Dissolve, // Dissolve
        Fade, // Fade*/
        UncoverLeft, // Uncover Left
        UncoverUp, // Uncover Up
        UncoverRight, // Uncover Right
        UncoverDown, // Uncover Down
        UncoverLeftUp, // Uncover Left Up
        UncoverRightUp, // Uncover Right Up
        UncoverLeftDown, // Uncover Left Down
        UncoverRightDown, // Uncover Right Down
        /*RandomBarsHorizontal, // Random Bars Horizontal
        RandomBarsVertical, // Random Bars Vertical
        StripsUpLeft, // Strips Up Left
        StripsUpRight, // Strips Up Right
        StripsDownLeft, // Strips Down Left
        StripsDownRight, // Strips Down Right
        StripsLeftUp, // Strips Left Up
        StripsRightUp, // Strips Right Up
        StripsLeftDown, // Strips Left Down
        StripsRightDown, // Strips Right Down
        WipeLeft, // Wipe Left
        WipeUp, // Wipe Up
        WipeRight, // Wipe Right
        WipeDown, // Wipe Down
        BoxOut, // Box Out
        BoxIn, // Box In*/
        FlyFromLeft, // Fly From Left
        FlyFromTop, // Fly From Top
        FlyFromRight, // Fly From Right
        FlyFromBottom, // Fly From Bottom
        FlyFromTopLeft, // Fly From Top Left
        FlyFromTopRight, // Fly From Top Right
        FlyFromBottomLeft, // Fly From Bottom Left
        FlyFromBottomRight, // Fly From Bottom Right
        /*PeekFromLeft, // Peek From Left
        PeekFromDown, // Peek From Down
        PeekFromRight, // Peek From Right
        PeekFromUp, // Peek From Up
        CrawlFromLeft, // Crawl From Left
        CrawlFromUp, // Crawl From Up
        CrawlFromRight, // Crawl From Right
        CrawlFromDown, // Crawl From Down
        ZoomIn, // Zoom In
        ZoomInSlightly, // Zoom In Slightly
        ZoomOut, // Zoom Out
        ZoomOutSlightly, // Zoom Out Slightly
        ZoomCenter, // Zoom Center
        ZoomBottom, // Zoom Bottom
        StretchAcross, // Stretch Across
        StretchLeft, // Stretch Left
        StretchUp, // Stretch Up
        StretchRight, // Stretch Right
        StretchDown, // Stretch Down
        Swivel, // Swivel
        Spiral, // Spiral
        SplitHorizontalOut, // Split Horizontal Out
        SplitHorizontalIn, // Split Horizontal In
        SplitVerticalOut, // Split Vertical Out
        SplitVerticalIn, // Split Vertical In
        FlashOnceFast, // Flash Once Fast
        FlashOnceMedium, // Flash Once Medium
        FlashOnceSlow, // Flash Once Slow
        Appear, // Appear
        CircleOut, // Circle Out
        DiamondOut, // Diamond Cut
        CombHorizontal, // Comb Horizontal
        CombVertical, // Comb Vertical
        FadeSmoothly, // Fade Smoothly
        Newsflash, // Newsflash
        PlusOut, // Plus Out
        PushDown, // Push Down
        PushLeft, // Push Left
        PushRight, // Push Right
        PushUp, // Push Up
        Wedge, // Wedge
        Wheel1Spoke, // Wheel 1 Spoke
        Wheel2Spokes, // Wheel 2 Spokes
        Wheel3Spokes, // Wheel 3 Spokes
        Wheel4Spokes, // Wheel 4 Spokes
        Wheel8Spokes, // Wheel 8 Spokes
        WheelReverse1Spoke, // Wheel Reverse 1 Spoke
        VortexLeft, // Vortex Left
        VortexUp, // Vortex Up
        VortexRight, // Vortex Right
        VortexDown, // Vortex Down
        RippleCenter, // Ripple Center
        RippleRightUp, // Ripple Right Up
        RippleLeftUp, // Ripple Left Up
        RippleLeftDown, // Ripple Left Down
        RippleRightDown, // Ripple Right Down
        GlitterDiamondLeft, // Glitter Diamond Left
        GlitterDiamondUp, // Glitter Diamond Up
        GlitterDiamondRight, // Glitter Diamond Right
        GlitterDiamondDown, // Glitter Diamond Down
        GlitterHexagonLeft, // Glitter Hexagon Left
        GlitterHexagonUp, // Glitter Hexagon Up
        GlitterHexagonRight, // Glitter Hexagon Right
        GlitterHexagonDown, // Glitter Hexagon Down
        GalleryLeft, // Gallery Left
        GalleryRight, // Gallery Right
        ConveyorLeft, // Conveyor Left
        ConveyorRight, // Conveyor Right
        DoorsVertical, // Doors Vertical
        DoorsHorizontal, // Doors Horizontal
        WindowVertical, // Window Vertical
        WindowHorizontal, // Window Horizontal
        WarpIn, // Warp In
        WarpOut, // Warp Out
        FlyThroughIn, // Fly Through In
        FlyThroughOut, // Fly Through Out
        FlyThroughInBounce, // Fly Through In Bounce
        FlyThroughOutBounce, // Fly Through Out Bounce
        RevealSmoothLeft, // Reveal Smooth Left
        RevealSmoothRight, // Reveal Smooth Right
        RevealBlackLeft, // Reveal Black Left
        RevealBlackRight, // Reveal Black Right
        Honeycomb, // Honeycomb
        FerrisWheelLeft, // Ferris Wheel Left
        FerrisWheelRight, // Ferris Wheel Right
        SwitchLeft, // Switch Left
        SwitchUp, // Switch Up
        SwitchRight, // Switch Right
        SwitchDown, // Switch Down
        FlipLeft, // Flip Left
        FlipUp, // Flip Up
        FlipRight, // Flip Right
        FlipDown, // Flip Down
        Flashbulb, // Flashbulb
        ShredStripsIn, // Shred Strips In
        ShredStripsOut, // Shred Strips Out
        ShredRectangleIn, // Shred Rectangle In
        ShredRectangleOut, // Shred Rectangle Out
        CubeLeft, // Cube Left
        CubeUp, // Cube Up
        CubeRight, // Cube Right
        CubeDown, // Cube Down
        RotateLeft, // Rotate Left
        RotateUp, // Rotate Up
        RotateRight, // Rotate Right
        RotateDown, // Rotate Down
        BoxLeft, // Box Left
        BoxUp, // Box Up
        BoxRight, // Box Right
        BoxDown, // Box Down
        OrbitLeft, // Orbit Left
        OrbitUp, // Orbit Up
        OrbitRight, // Orbit Right
        OrbitDown, // Orbit Down*/
        PanLeft, // Pan Left
        PanUp, // Pan Up
        PanRight, // Pan Right
        PanDown, // Pan Down
    }
}
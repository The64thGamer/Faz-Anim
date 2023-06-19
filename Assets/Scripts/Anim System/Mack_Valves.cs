using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mack_Valves : MonoBehaviour
{
    public float PSI = 80f;
    public bool[] topDrawer = new bool[95];
    public bool[] bottomDrawer = new bool[97];

    /*Bit Charts:
     * 
     * FNAF1
     * 
     * TOP DRAWER
     *
     *1         Jaw                 Freddy
     *2         Head Up             Freddy 
     *3         Eyes Right          Freddy
     *4         Eyes Left           Freddy
     *5         Eyelids Right       Freddy
     *6         Eyelids Left        Freddy
     *7         Shoulder Twist R    Freddy
     *8         Shoulder Twist L    Freddy
     *9         Shoulder Up R       Freddy
     *10        Shoulder Up L       Freddy
     *11        Arm Twist R         Freddy
     *12        Arm Twist L         Freddy
     *13        Arm Bend R          Freddy
     *14        Arm Bend L          Freddy
     *15        Wrist Twist R       Freddy
     *16        Wrist Twist L       Freddy
     *17        Wrist Out R         Freddy
     *18        Wrist Out L         Freddy
     *19        Head Turn R         Freddy
     *20        Head Turn L         Freddy
     *21        Body Turn R         Freddy
     *22        Body Turn L         Freddy
     *23        Body Lean           Freddy
     *24        Foot Tap            Freddy
     *25        Shoulder Twist L    Chica
     *26        Arm Twist L         Chica
     *27        Twist               Cupcake
     *28        Eyelid R            Chica
     *29        Eyelid L            Chica
     *30        Jaw                 Chica
     *31        Eyes Right          Chica
     *32        Eyes Left           Chica
     *33        Jaw                 Bonnie
     *34        Head Up             Bonnie 
     *35        Eyes Right          Bonnie
     *36        Eyes Left           Bonnie
     *37        Eyelids Right       Bonnie
     *38        Eyelids Left        Bonnie
     *39        Shoulder Twist R    Bonnie
     *40        Shoulder Twist L    Bonnie
     *41        Shoulder Up R       Bonnie
     *42        Ear R               Bonnie
     *43        Arm Twist R         Bonnie
     *44        Arm Twist L         Bonnie
     *45        Ear L               Bonnie
     *46        Arm Bend L          Bonnie
     *47        Wrist Twist R       Bonnie
     *48        Wrist Twist L       Bonnie
     *49        Wrist Out R         Bonnie
     *50
     *51        Head Turn R         Bonnie
     *52        Head Turn L         Bonnie
     *53        Body Turn R         Bonnie
     *54        Body Turn L         Bonnie
     *55        Body Lean           Bonnie
     *56        Body Lean           Chica
     *57        Body Turn R         Chica
     *58        Body Turn L         Chica
     *59        Head Turn R         Chica
     *60        Head Turn L         Chica
     *61        Head Up             Chica
     *62        Eyes R              Chica
     *63        Eyes L              Chica
     *64        Eyelid R            Chica
     *65        Eyelid L            Chica
     *66        Shoulder Twist R    Chica
     *67        Shoulder Up R       Chica
     *68        Arm Twist R         Chica
     *69        Forearm Bend R      Chica
     *70        Wrist Twist R       Chica
     *71        Wrist Bend R        Chica
     *72        Eyes Right          Cupcake
     *73        Eyes Left           Cupcake
     *74        Eyelid R            Cupcake
     *75        Eyelid L            Cupcake
     *76
     *77
     *78
     *79
     *80        Jaw                 Parrot
     *81        Arm Twist R         Foxy
     *82        Head Up             Foxy
     *83        Eyes R              Foxy
     *84        Eyes L              Foxy
     *85        Eyelid R            Foxy
     *86        Eyepatch            Foxy    
     *87        Jaw                 Foxy
     *88        Arm Twist L         Foxy
     *89        Main OPN            Curtains
     *90        Main CLS            Curtains
     *91        Side OPN            Curtains
     *92        Side CLS            Curtains
     *93        Cage                Parrot
     *94        Body Lean F         Parrot
     *95        Body Lean B         Parrot
     *96        Arms                Parrot
     *97        Head Tip F          Parrot
     *98        Head Tip B          Parrot
     *99        Arm Up R            Foxy
     *100       Arm Up L            Foxy
     *101       Forearm Twist R     Foxy
     *102       Forearm Twist L     Foxy
     *103       Forearm Bend R      Foxy
     *104       Forearm Bend L      Foxy
     *105       Wrist Twist R       Foxy
     *106       Wrist Twist L       Foxy
     *107       Wrist Bend R        Foxy
     *108       Wrist Bend L        Foxy
     *109       Head Turn R         Foxy
     *110       Head Turn L         Foxy
     *111       Body Turn R         Foxy
     *112       Body Turn L         Foxy
     *113       Body Lean           Foxy
     *114       Foot Tap R          Foxy
     *115       Foot Tap L          Foxy
     *
     *BOTTOM DRAWER
     *
     *1         Freddy              Spotlights
     *2         Purple              Spotlights
     *3         Green               Spotlights
     *4         Orange              Spotlights
     *5         Lower 1             Stage Lights
     *6         Lower 2             Stage Lights
     *7         Lower 3             Stage Lights
     *6         Lower 4             Stage Lights
     *9         Lower 5             Stage Lights    
     *10        Lower 6             Stage Lights    
     *11        Lower 7             Stage Lights
     *12        Bonnie              Spotlights
     *13        Chica               Spotlights
     *14        Red                 Flood Lights
     *15        Yellow              Flood Lights
     *16        Green               Flood Lights
     *17        Blue                Flood Lights
     *18        Sun                 Spotlights
     *19        Speaker Red         Stage Lights
     *20        Backdrop Blue       Stage Lights
     *21        Fiber Optic Stars   Stage Lights
     *22        Foxy                Spotlights
     *23        Main Mood Lights    Stage Lights
     *24        Side Mood Lights    Stage Lights
     *25        Freddy              Dual Pressure
     *26        Bonnie              Dual Pressure
     *27        Chica               Dual Pressure
     *28        Foxy                Dual Pressure
     *29        Foxy Upper          Spotlights
     *30        Foxy Chand 1        Stage Lights
     *31        Foxy Chand 2        Stage Lights
     *32        Deck Day            Stage Lights
     *33        Deck Sunset         Stage Lights
     *34        Deck Night          Stage Lights
     *35        Deck Flash          Stage Lights
     *36        Bird                Spotlights
     *37        Red                 Spotlights
     *38        Yellow              Spotlights
     *39        Blue                Spotlights
     *40        Main Off            TVs
     *41        Side Off            TVs
     *42        Freddy Movie        Spotlights
     *43        Freddy Movie Upper  Spotlights
     *44        
     *45        Snacks              Stage Lights
     *
     *
     *
     *
     *
     *
     *
     *
     *
     *
     *
     *
     *
     * * FNAF2
     * 
     * TOP DRAWER
     *
     *1         Jaw                 Bonnie
     *2         
     *
     *BOTTOM DRAWER
     *
     *1         Bonnie              Spotlights
     *2         
     *3         
     *4         
     *5         
     *6         
     *7         
     *6         
     *9           
     *10          
     *11        
     *12        
     *13        
     *14        
     *15        
     *16        
     *17        
     *18        
     *19        
     *20        
     *21        
     *22        
     *23        
     *24        
     *25        Freddy              Dual Pressure
     *26        Bonnie              Dual Pressure
     *27        Chica               Dual Pressure
     *28        Foxy                Dual Pressure
     *29        Balloon Boy         Dual Pressure
     *30        
     *31        
     *32        
     *33        
     *34        
     *35        
  */




}

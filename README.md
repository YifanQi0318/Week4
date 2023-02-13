• What are the benefits of creating a Dictionary for the card images instead of storing the card images 
as part of our instances of the Card class? What are the potential benefits if we make the card image 
a field of the Card class instead? 

A：Creating a Dictionary for the card images can be more convenient for management and reference instead of crude overall handling. If you create an image, you can only manage it in terms of naming.
If you make an image a field, you can better reference the name.


 
• How else could we use a Dictionary to improve our program? What else do we need to refer to 
regularly in this program that would benefit from a list of key/value pairs? 

A：Add more resources to the dictionary, such as the shader change of the card (when this effect is triggered, the image changes)



 
• CardTable is the “display” class for this project. It controls the basics of user input and output. 
Bearing in mind that we are going to completely change the display aspect of this project to involve 
card graphics and point-and-click choices, what should be moved from other classes to this class?  
o For example, some things in this class could be made a lot more generic, moving all 
validation to Game.cs so that the CardTable only prints things to screen and reads data, 
while the Game.cs class takes care of ensuring the user is following the rules. Why would we 
do this? 


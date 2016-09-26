# Silverlight Clock 2

### Description
I started working on enhancing the previous Silverlight 2.0 clock I had made, en then I saw a Flash implementation of another creative approach of a clock and wondered if I could do that in Silverlight.
Needless to say, I started working on it immediately and below is the result. This is another experiment that I thought I could do in just a few hours and it ended up taking me a bit more. Mostly because of the uneven conversions between the units and the different sizes between the boxes.

Anyways, in a nutshell here is my approach: I have a very simple XAML file with only defining seven empty canvasses as placeholders, organized from top to bottom by a StackPanel. I will create and populate every single object dynamically and on the fly: I have a single method (CreateBasicCancas) that generates all the textBoxes for each individual gear. Then I have a single Storyboard that iterates every millisecond and that storyboard calls one single method (MoveGears) for all gears. It will loop through the collection of Textboxes of a particular canvas and adjust their position accordingly.
There is an occasional bug with the days not rendering properly, but overall it works decently….

### usage
Clone and run **Silverlight_Clock_2_TestPage.html** (ensure you have a browser capable of rendering Silverlight 2.0)

![clock2](http://i.imgur.com/6RpgokR.png)![clock2](http://i.imgur.com/9XlFow8.png)

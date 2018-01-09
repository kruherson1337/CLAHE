# CLAHE
Contrast Limited Adaptive Histogram Equalization C# .NET - 2018

C# implementation of CLAHE algorithm.

More on algorithm find here https://en.wikipedia.org/wiki/Adaptive_histogram_equalization#Contrast_Limited_AHE

Requirements:
  - Target framework: .NET Framework 4.6.1
  - C# WPF App

Program supports features:
  - Loading multi channel images (1,3,4)
  - Processing each channel on seperated thread
  - Histogram Equalization
  - Adaptive Histogram Equalization
  - Contrast Limited Histogram Equalization
  - Contrast Limited Adaptive Histogram Equalization
   
Tutorial:
  - Load image (double left click on image to load)
  - Select algorithm
  - Save image (right click on processed image to save)
  
References:
  CLAHE article =>  http://www.cs.unc.edu/Research/MIDAG/pubs/papers/Adaptive%20Histogram%20Equalization%20and%20Its%20Variations.pdf  
  CLAHE article =>  https://digitalcommons.unf.edu/cgi/viewcontent.cgi?referer=&httpsredir=1&article=1264&context=etd
  CLAHE implementation =>   https://github.com/cjaques/clahe_net/tree/master/Clahe_w
  Histogram Equalization implementation =>  https://github.com/sairamganti/Csharp/tree/master/histogram%20equalization
  Histogram Equalization demonstration => https://www.youtube.com/watch?v=eNBZI-qYhpg

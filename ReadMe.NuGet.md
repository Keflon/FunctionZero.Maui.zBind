`FunctionZero.Maui.zBind` is a `xaml` markup extension for **MAUI** that allows you to bind directly to an `expression` 

Install version 8.0.0 and above for .NET 8.  


If you want to do things like this: (note the expression is enclosed inside quotes)
```xaml
<StackLayout IsVisible="{z:Bind '(Item.Count != 0) AND (Status == \'Administrator\')'}" > ...
```

1. Install `FunctionZero.Maui.zBind` to your shared project
2. add  `xmlns:z="clr-namespace:FunctionZero.Maui.zBind.z;assembly=FunctionZero.Maui.zBind"
`  
To your `xaml` page (or let Visual Studio do it for you)

Head over [here](https://github.com/Keflon/FunctionZero.Maui.zBind) for 
source code, documentation and a sample application

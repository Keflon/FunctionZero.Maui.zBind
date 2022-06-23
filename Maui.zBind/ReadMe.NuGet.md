`FunctionZero.Maui.zBind` is a `xaml` markup extension for **MAUI** that allows you to bind directly to an `expression` 

If you want to do things like this: (note the expression is enclosed inside quotes)
```xaml
<StackLayout IsVisible="{z:Bind '(Item.Count != 0) AND (Status == \'Administrator\')'}" > ...
```

1. Install `FunctionZero.Maui.zBind` to your shared project
2. add  `xmlns:z="clr-namespace:FunctionZero.zBind.z;assembly=FunctionZero.zBind"`  
To your `xaml` page (or let Visual Studio do it for you)

Head over [here](https://github.com/Keflon/FunctionZero.zBindTestApp) for 
source code, documentation and a sample application

# FunctionZero.Maui.zBind
`FunctionZero.Maui.zBind` contains an alternative to `Microsoft.Maui.Controls.Binding` and allows DataBinding to an Expression  

NuGet package [here](https://www.nuget.org/packages/FunctionZero.Maui.zBind)  
Xamarin version [here](https://github.com/Keflon/FunctionZero.zBindTestApp)  
Xamarin NuGet [here](https://www.nuget.org/packages/FunctionZero.zBind)  

## Contents
1. [Quickstart](#Quickstart)
1. [z:Bind](#zBind)
1. [The Great Escape, and other cautionary tales](#The-Great-Escape-and-other-cautionary-tales)
1. [z:Function](#zFunction)
1. [z:TapTrigger](#zTapTrigger)
1. [z:EdgeTrigger](#zEdgeTrigger)
1. [z:Latch](#zLatch)
1. [Advanced Usage - Functions, aliases and operator-overloads](#Advanced-Usage-Functions-aliases-and-operator-overloads)

## Quickstart
### Setup
1. Use the package manager to add the NugetPackage `FunctionZero.Maui.zBind`
1. Add a namespace alias to your xaml, like this:
```xaml 
    xmlns:z="clr-namespace:FunctionZero.Maui.zBind.z;assembly=FunctionZero.Maui.zBind"
```
### Getting Started ...
#### z:Bind - animate an Image based on 'Count' in your ViewModel ...
```csharp
<Image Source="dotnet_bot.png"
       TranslationX="{z:Bind Sin(Count / 25.0) * 100}"
       TranslationY="{z:Bind Cos(Count / 15.0) * 100}"
       Rotation="{z:Bind Sin(Count / 5.0) * 20}"
/>
```

#### z:TapTrigger - modify a ViewModel property when tapped ...
```csharp
<Button Text="Reset 'Count' in the ViewModel">
    <Button.Behaviors>
        <z:TapTrigger TapAction="{z:Function 'Count = 0'}"/>
    </Button.Behaviors>
</Button>
```
#### z:EdgeTrigger - when a Condition changes, execute a Rising or Falling Expression ...
```csharp
<ContentPage.Behaviors>
    <z:EdgeTrigger 
        Condition="{z:Bind '(Value LT 0.5)', Source={x:Reference TheSlider}}" 
        Rising="{z:Function 'RotationY = 90', Source={x:Reference TheImage}}"
        Falling="{z:Function 'RotationY = 180', Source={x:Reference TheImage}}" />
</ContentPage.Behaviors>
```
## z:Bind
Used to DataBind to an *Expression*. If the *Expression* is just a property-name then it is equivalent to `Binding`  
A `z:Bind` automatically updates if any referenced properties notify they have changed  
### Usage
`TargetProperty = { z:Bind <some expression>   [, Source=<some source>] }`
### Source
Just like a standard `Binding`, the *data-source* is the current *BindingContext*, unless `Source` is specified.  
If this *data-source* supports `INotifyPropertyChanged`, changes will be tracked.
### z:Bind Expressions
|Sample Expression|Source|Notes|
|--|:--:|--|
|`{z:Bind Count}`|BindingContext| Bind to `Count`, same as `Binding`|
|`{z:Bind Count * 2}`|BindingContext| Bind to an expression that yields Count * 2|
|`{z:Bind '(Delta.X &lt; 0.2) &amp;&amp; (Delta.X &gt; -0.2)' }`|BindingContext| True if (Delta.X < 0.2) && (Delta.X > -0.2)|
|`{z:Bind '(Delta.X LT 0.2) AND (Delta.X GT -0.2)' }`|BindingContext| As above, using *aliases* instead of escape-sequences|
|`{z:Bind (Count * 2) LT 10}`|BindingContext| True if (Count * 2) < 10|
|`{z:Bind Sin(Count / 25.0)}`|BindingContext| Calls a _function_ (see below)|
|`{z:Bind 'Value LT 0.2', Source={x:Reference MySlider}}`|An *Element* called MySlider|True if `MySlider.Value` < 0.2|

### z:Bind Examples
#### Hide a `StackLayout` if `Things.Count != 0` ...
```xaml
<StackLayout IsVisible="{z:Bind '(Things.Count != 0)'}" ... > ...
```
#### Animate an `Image` based on a `Count` property ...
```xaml
<Image TranslationX="{z:Bind Sin(Count / 25.0) * 100}" 
       TranslationY="{ ... }" />
```
#### Scale a `Label` based on the `Value` of a `Slider` Control ...
```xaml
<Label Scale="{z:Bind Value * 3 + 0.1, Source={x:Reference TheSlider}}" > ...
```














## The Great Escape, and other cautionary tales
As with `xaml`, string literals can be enclosed within 'single quotes' or "double quotes" with appropriate use 
of `xml` escape-sequences.
### Commas
If your _expression string_ has commas in it, you must hide them from the xaml parser, otherwise `z:Bind` etc. will be given an incomplete string and things won't work as expected. 

You can do this by enclosing the string inside quotes, like this:
```xaml
Something="{z:Bind 'SomeFunction(param1, param2)'}"
```
or this
```xaml
Something="{z:Bind \"SomeFunction(param1, param2)\"}"
```
and so on
### Strings
If your _expression string_ has string literals in it, you must 'escape' them, otherwise `z:Bind` etc. will be given an incorrect string and things won't work as expected.   
For example:
```xml
"{z:Bind Status == \'Administrator\'}"
```
or this
```xml
"{z:Bind Status == '\'Administrator\''}"
```
and so on
### Long form
If your expression is getting bogged down in escape-sequences and commas and quotes, or if that's just the way you roll, 
you can use the long-form of expressing a `z:Bind` expression:

```xml
<Label Text="{z:Bind '\'Score: \'+ Count + \' points\''}"
```
becomes
```xml
<Label>
    <Label.Text>
        <z:Bind>
            'Score: '+ Count + ' points'
        </z:Bind>
    </Label.Text>
</Label>
```

### Casting
Functions and assignments will try to cast to the correct type, so `Sin(someFloat)` will work despite `Sin` requiring a *double*   
If you want to explicitly cast you can do so like this: `Sin((Double)someFloat)`  
See [`ExpressionParserZero.Operands.OperandType`](https://github.com/Keflon/FunctionZero.ExpressionParserZero#casting) for details.

### Short-circuit
Just like `c#`, the underlying expression parser supports short-circuit, so expressions like 
`(thing != null) AND (thing.part == 5)` will work even if `thing` is `null`
### Errors
Error reporting is quite good, so check the debug output if things aren't working as expected.

### Aliases supported to simplify xaml

|Alias|Operator|
|--|:--:|
|NOT|!|
|MOD|%|
|LT|<|
|GT|>|
|GTE|>=|
|LTE|<=|
|BAND|&| 
|XOR|^| 
|BOR|\||
|AND|&&|
|OR| \|\||

### Supported value types
All csharp value-types and their `nullable` variants

### Supported reference types
`string`, `object`

## z:Function
`z:Function` is similar to, and has the same syntax as `z:Bind`  
It is different because it does not self-evaluate and instead relies on its consumer to ask it to evaluate  
`z:Function` can be consumed by `TapTrigger`, `EdgeTrigger` and `Latch`
### Usage
```csharp
TargetProperty = "{ z:Function <some expression>   [, Source=<some source>] }"
```
Where *&lt;some expression&gt;* is any valid expression, making use of properties on the *BindingSource*  
Typically you would put either an assignment into a z:Function
```xml
TapAction="{z:Function 'Things.TapCount = Things.TapCount + 1'}"
```
or a function call
```xml
TapAction="{z:Function 'OpenUrl(Customer.LoginUrl, Customer.UserName)'}"
```
or both ...
```xml
TapAction="{z:Function 'Things.TapCount = Things.TapCount + 1, OpenUrl(Customer.LoginUrl)'}"
```

## z:TapTrigger
`z:TapTrigger` is a *Behaviour* that evaluates a `z:Function` when its *host* is tapped  
### Usage
```csharp
<z:TapTrigger TapAction="{z:Function '<some expression>'}"/>
```  
  For example:
```csharp
<Button Text="Reset 'Count' using a TapTrigger"
    <Button.Behaviors>
        <z:TapTrigger TapAction="{z:Function 'Count = 0'}"/>
    </Button.Behaviors>
</Button>
```

## z:EdgeTrigger
z:EdgeTrigger is a *Behaviour* that evaluates a 'Rising' or 'Falling' `z:Function` when a *Condition* changes  
### Usage
```xml
<ContentPage.Behaviors>
    <z:EdgeTrigger 
        Condition="<some z:Bind>" 
        Rising="<some z:Function>"
        Falling="<some z:Function>" />
</ContentPage.Behaviors>
```
For example, to show or hide UI based on a *player data* found in the *ViewModel*
```xml
<ContentPage.Behaviors>
    <z:EdgeTrigger 
        Condition="{z:Bind '(Player.Score GTE 50) AND (Player.IsVerified == true)'}" 
        Rising="{z:Function 'IsVisible=true', Source={x:Reference TheAchevementUi}}"
        Falling="{z:Function 'IsVisible=false', Source={x:Reference TheAchevementUi}}" />
</ContentPage.Behaviors>
```



## z:Latch
Coming s<sub>p</sub>oon <sup>*</sup>  
### Usage
Coming s<sub>p</sub>oon <sup>*</sup>  





## Advanced Usage - Functions, aliases and operator-overloads
 `z:Bind` uses [`FunctionZero.ExpressionParserZero`](https://github.com/Keflon/FunctionZero.ExpressionParserZero) to do 
 the heavy lifting, so take a look at the [documentation](https://github.com/Keflon/FunctionZero.ExpressionParserZero)
if you want to take a deeper dive. Here is a taster ...
### Functions
`Sin`, `Cos` and `Tan` are registered by default, as are the _aliases_ listed above.
```xaml
<Label TranslationX="{z:Bind Sin(Count / 25.0) * 100.0}" ...
```
Suppose you wanted a new function to to do a linear interpolation between two values, like this:  
*(Spoiler: Lerp is also pre-registered)*
```csharp
float Lerp(float a, float b, float t)
{
  return a + t * (b - a);
}
```
For use like this:
```xaml
<Label Rotation={z:Bind Lerp(0, 360, rotationPercent / 100.0)} ...
```
First you will need a reference to the default ExpressionParser
```csharp
var ep = ExpressionParserFactory.GetExpressionParser();
```
Then _register_ a _function_ that takes 3 parameters:
```csharp
ep.RegisterFunction("Lerp", DoLerp, 3);
```
Finally write the DoLerp method referenced above.
```csharp
private static void DoLerp(Stack<IOperand> stack, IBackingStore backingStore, long paramCount)
{
    // Pop the correct number of parameters from the operands stack, ** in reverse order **
    // If an operand is a variable, it is resolved from the backing store provided
    IOperand third = OperatorActions.PopAndResolve(operands, backingStore);
    IOperand second = OperatorActions.PopAndResolve(operands, backingStore);
    IOperand first = OperatorActions.PopAndResolve(operands, backingStore);

    float a = Convert.ToSingle(first.GetValue());
    float b = Convert.ToSingle(second.GetValue());
    float t = Convert.ToSingle(third.GetValue());

    // The result is of type float
    float result = a + t * (b - a);

    // Push the result back onto the operand stack
    stack.Push(new Operand(-1, OperandType.Float, result));
}
```

### Aliases
Get a reference to the default ExpressionParser:
```csharp
var ep = ExpressionParserFactory.GetExpressionParser();
```
Then register a new `operator` and use the existing `matrix` for `&&`

(See the `ExpressionParserZero` [source and documentation](https://github.com/Keflon/FunctionZero.ExpressionParserZero) for more details)
```csharp
ep.RegisterOperator("AND", 4, LogicalAndMatrix.Create());
```
### Overloads
Suppose you want to add a `long` to a `string`

Get a reference to the default ExpressionParser:
```csharp
var ep = ExpressionParserFactory.GetExpressionParser();
```
Then simply register the overload like this
```csharp
// Overload that will allow a long to be appended to a string
// To add a string to a long you'll need to add another overload
ep.RegisterOverload("+", OperandType.String, OperandType.Long, 
    (left, right) => new Operand(OperandType.String, (string)left.GetValue() + ((long)right.GetValue()).ToString()));
```
and to add a `string` to a `long`:
```csharp
// Overload that will allow a string to be appended to a long
// To add a long to a string you'll need to add another overload
ep.RegisterOverload("+", OperandType.Long, OperandType.String, 
    (left, right) => new Operand(OperandType.String, (long)left.GetValue() + ((string)right.GetValue()).ToString()));
```

Putting the above into action, you can then start to really have some fun
```xml
<Label 
    Text="{z:Bind '\'Player 1 score \' + playerOne.Score + \'points\''}
    Rotation="{z:Bind 'Lerp(0, 360, rotationPercent / 100.0)'"}
/>
```





<sup>* There is no spoon.<sup>  

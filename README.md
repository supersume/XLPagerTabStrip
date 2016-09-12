# XLPagerTabStrip
Xamarin port of the iOS library [XLPagerTabStrip](https://github.com/xmartlabs/XLPagerTabStrip) by [XMARTLABS](http://xmartlabs.com/).

Android [PagerTabStrip](http://developer.android.com/reference/android/support/v4/view/PagerTabStrip.html) for iOS!

**XLPagerTabStrip** is a _Container View Controller_ that allows us to switch easily among a collection of view controllers. Pan gesture can be used to move on to next or previous view controller. It shows a interactive indicator of the current, previous, next child view controllers.

Dou you use the library? You can check us out on [twitter](https://twitter.com/supersume)

# Pager types
Currently the library supports only one pager type

## Button Bar
This is likely to be the most common pager type. It's used by many well known apps such as instagram, youtube, skype and many others.

![Screenshot](https://github.com/supersume/XLPagerTabStrip/blob/master/Simulator.png?raw=true)

# Usage
Basically we just need to provide the list of child view controllers to show and these view controllers should provide the information (title or image) that will be shown in the associated indicator.

Let's see the steps to do this:

##### Choose which type of pager we want to create

First we should choose the type of pager we want to create, depending on our choice we will have to create a view controller that inherits from one of the following controllers: `TwitterPagerTabStripViewController`, `ButtonBarPagerTabStripViewController`, `SegmentedPagerTabStripViewController`,`BarPagerTabStripViewController`.

> All these build-in pager controllers extend from the base class PagerTabStripViewController. You can also make your custom pager controller by extending directly from PagerTabStripViewController in case no pager menu type fits your needs.

```c#
using XLPagerTabStrip;

public class MyPagerTabStripName: ButtonBarPagerTabStripViewController 
{
  ..
}
```

##### Provide the view controllers that will appear embedded into the PagerTabStrip view controller

You can provide the view controllers by overriding `CreateViewControllersForPagerTabStrip(PagerTabStripViewController pagerTabStripController)` method.

```c#
public override UIViewController[] CreateViewControllersForPagerTabStrip(PagerTabStripViewController pagerTabStripViewController) 
{
  return new UIViewController[] { MyEmbeddedViewController(), MySecondEmbeddedViewController() };
}
```

> The method above is the only method declared in `IPagerTabStripDataSource` interface. We don't need to explicitly conform to it since base pager class already does it.


##### Provide information to show in each indicator

Every UIViewController that will appear within the PagerTabStrip needs to provide either a title or an image.
In order to do so they should implement the `IIndicatorInfoProvider` interface method `IndicatorInfoForPagerTabStrip(PagerTabStripViewController pagerTabStripController)`
 which provides the information required to show the PagerTabStrip menu (indicator) associated with the view controller.

```c#
public class MyEmbeddedViewController: UITableViewController, IIndicatorInfoProvider
{
  public IndicatorInfo IndicatorInfoForPagerTabStrip(PagerTabStripViewController pagerTabStripController)     {
    return new IndicatorInfo("My Child title");
  }
}
```

## Customization

##### Pager Behaviour

The pager indicator can be updated progressive as we swipe or at once in the middle of the transition between the view controllers.
By setting up `pagerBehaviour` property we can choose how the indicator should be updated.

```c#
public PagerTabStripBehaviour pagerBehaviour;
```

```c#
public class PagerTabStripBehaviour 
{
    public bool? SkipIntermediateViewControllers { get; set; };
    public bool? ElasticIndicatorLimit { get; set; };
}
```

Default Values:
```c#
// Twitter Type
PagerTabStripBehaviour.Create(skipIntermediteViewControllers: true);
// Segmented Type
PagerTabStripBehaviour.Create(skipIntermediteViewControllers: true);
// Bar Type
PagerTabStripBehaviour.Create(skipIntermediteViewControllers: true, elasticIndicatorLimit: true);
// ButtonBar Type
PagerTabStripBehaviour.Create(skipIntermediteViewControllers: true, elasticIndicatorLimit: true);
```
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

**Choose which type of pager we want to create**

First we should choose the type of pager we want to create, depending on our choice we will have to create a view controller that inherits from one of the following controllers: `TwitterPagerTabStripViewController`, `ButtonBarPagerTabStripViewController`, `SegmentedPagerTabStripViewController`,`BarPagerTabStripViewController`.

> All these build-in pager controllers extend from the base class PagerTabStripViewController. You can also make your custom pager controller by extending directly from PagerTabStripViewController in case no pager menu type fits your needs.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml.Data;
//  
/// <summary> 
/// Using the C# enumUtilities class(es) to bind an enum to a ComboBox 
/// </summary> 
/// <remarks> 
/// The enumUtilities classes make it simple to bind a public property of  
/// an enum you specify to a ComboBox. There is lots of advice for this 
/// on the internet, but much of it doesn't work for Universal Apps. 
///  
/// Steps: 
/// Update your project: 
/// 1. Include EnumUtiliies.cs in your project 
///  
/// Update your MainPage.xaml.cs file: 
/// 1. add using enumUtilities; 
/// 2. add a de-templatized version of EnumValueConverter 
///     public class MyEnumExampleConverter : EnumValueConverter<MainPage.MyEnumExample> { } 
/// 3. add your enum to the MainPage class 
/* 
       public enum MyEnumExample 
        { 
            [enumUtilities.Display("")] // Items with a blank Display("") are not listed in the UI 
            Unknown, 
            [enumUtilities.Display("The Test enum")] 
            Test, 
            [enumUtilities.Display("Check is being used")] 
            Check, 
            Validate // Items with nothing in the Display() property will be listed in the UI. 
        }; 
*/
/// 4. Make sure you've set your DataContext: this.DataContext = this; 
/// 5. Add the property to bind to.  It doesn't have to be INotifyPropertyChanged property. 
/* 
        private MyEnumExample _CurrExampleEnum = MyEnumExample.Check; 
        public MyEnumExample CurrExampleEnum 
        { 
            get { return _CurrExampleEnum; } 
            set 
            { 
                if (value == _CurrExampleEnum) return; 
                _CurrExampleEnum = value; 
                OnPropertyChanged(); 
            } 
        } 
*/
/// Update your MainPage.xaml file 
/// 1. Add a namespace xmlns:enumUtilities="using:enumUtilities" 
/// 2. Add a resource to your Page: 
/*
     <Page.Resources> 
       <!-- You have to compile MainPage.Xaml.Cs for VS to find the local: class. --> 
        <local:MyEnumExampleConverter x:Key="MyEnumExampleConverter" /> 
    </Page.Resources> 
*/
/// 3. Add a ComboBox with an ItemTemplate 
/* 
             <ComboBox  Name="myComboBox" 
                       ItemsSource="{Binding Source={StaticResource MyEnumExampleConverter}, Path=EnumValues}" 
                      SelectedItem="{Binding CurrExampleEnum, Mode=TwoWay}"> 
                <!-- In order to display the strings from the DisplayAttribute you need to add a converter. --> 
                <ComboBox.ItemTemplate> 
                    <DataTemplate> 
                        <TextBlock FontSize="14" Text="{Binding Converter={StaticResource MyEnumExampleConverter}}" /> 
                    </DataTemplate> 
                </ComboBox.ItemTemplate> 
            </ComboBox> 
*/
/// </remarks> 
namespace enumUtilities
{
    [AttributeUsage(AttributeTargets.All)]
    public class DisplayAttribute : System.Attribute
    {
        public string Name { get; private set; }

        public DisplayAttribute(string name)
        {
            this.Name = name;
        }
    }

    public class EnumValueConverter<T> : IValueConverter
    {
        // 
        // Generates a cached list of enum values; items with blank DisplayAttribute  
        // values are ignored. This item is placed here for convenience; you can put  
        // this value anywhere. 
        // 
        private IList<T> _EnumValues = null;
        private void MakeEnumValues()
        {
            if (_EnumValues != null) return;
            _EnumValues = Enum.GetValues(typeof(T)).Cast<T>().Where((s) => {
                var name = GetDisplayAttribute(s);
                var retval = (name == null) || (name != "");
                return retval;
            }).ToList();
        }
        public IList<T> EnumValues { get { MakeEnumValues(); return _EnumValues; } }

        private string GetDisplayAttribute(T vv)
        {
            // Note: the GetTypeInfo only works when you have a using System.Reflection; 
            var cda = vv.GetType().GetTypeInfo().GetDeclaredField(vv.ToString())
                 .GetCustomAttribute<DisplayAttribute>();
            return cda?.Name;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is T)
            {
                var name = GetDisplayAttribute((T)value);
                if (name != null) return name;
            }
            return value.ToString().Replace("_", " ");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
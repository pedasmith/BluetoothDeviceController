# PageXaml FileName=[[CLASSNAME]]Page.xaml DirName=SpecialtyPages SuppressFile=:SuppressXAML:
```
<Page
    x:Class="BluetoothDeviceController.SpecialtyPages.[[CLASSNAME]]Page"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="using:BluetoothDeviceController.SpecialtyPages"
    xmlns:controls="using:Microsoft.Toolkit.Uwp.UI.Controls"
    xmlns:charts="using:BluetoothDeviceController.Charts"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d"
    Background="{ThemeResource ApplicationPageBackgroundThemeBrush}">
    <Page.Resources>
        <Style TargetType="Button">
            <Setter Property="MinWidth" Value="60" />
            <Setter Property="VerticalAlignment" Value="Bottom" />
            <Setter Property="FontFamily" Value="Segoe UI,Segoe MDL2 Assets" />
            <Setter Property="Margin" Value="10,5,0,0" />
        </Style>
        <Style TargetType="Slider">
            <Setter Property="MinWidth" Value="200" />
            <Setter Property="FontFamily" Value="Segoe UI,Segoe MDL2 Assets" />
            <Setter Property="Margin" Value="10,5,10,0" />
        </Style>
        <Style TargetType="Line">
            <Setter Property="Margin" Value="0,15,0,0" />
            <Setter Property="Stroke" Value="ForestGreen" />
        </Style>
        <Style x:Key="TitleStyle" TargetType="TextBlock">
            <Setter Property="FontSize" Value="30" />
        </Style>
        <Style x:Key="HeaderStyle" TargetType="TextBlock">
            <Setter Property="FontSize" Value="20" />
        </Style>
        <Style x:Key="HeaderStyleExpander" TargetType="controls:Expander">
            <Setter Property="MinWidth" Value="550" />
            <Setter Property="HorizontalAlignment" Value="Left" />
            <Setter Property="HorizontalContentAlignment" Value="Left" />
        </Style>
        <Style x:Key="SubheaderStyle" TargetType="TextBlock">
            <Setter Property="FontSize" Value="16" />
        </Style>
        <Style x:Key="AboutStyle" TargetType="TextBlock">
            <Setter Property="FontSize" Value="12" />
            <Setter Property="TextWrapping" Value="Wrap" />
        </Style>
        <Style x:Key="ChacteristicListStyle" TargetType="StackPanel">
            <Setter Property="Background" Value="WhiteSmoke" />
            <Setter Property="Margin" Value="18,0,0,0" />
        </Style>
        <Style x:Key="HEXStyle" TargetType="TextBox">
            <Setter Property="MinWidth" Value="90" />
            <Setter Property="FontSize" Value="12" />
            <Setter Property="Margin" Value="5,0,0,0" />
        </Style>
        <Style x:Key="TableStyle" TargetType="controls:DataGrid">
            <Setter Property = "Background" Value="BlanchedAlmond" />
            <Setter Property = "FontSize" Value="12" />
            <Setter Property = "Height" Value="200" />
            <Setter Property = "HorizontalAlignment" Value="Center" />
            <Setter Property = "Width" Value="500" />
        </Style>
    </Page.Resources>
    
    <StackPanel>
        <Grid MaxWidth="550" HorizontalAlignment="Left">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*" />
                <ColumnDefinition Width="auto" />
            </Grid.ColumnDefinitions>
            <StackPanel Grid.Column="0">
                <TextBlock Style="{StaticResource TitleStyle}">[[DeviceName]] device</TextBlock>
                <TextBlock Style="{StaticResource AboutStyle}">
                    [[DESCRIPTION]]
                </TextBlock>
            </StackPanel>
            <controls:ImageEx Grid.Column="1" Style="{StaticResource ImageStyle}"  Source="/Assets/DevicePictures/[[CLASSNAME]].PNG" />
        </Grid>
        <ProgressRing x:Name="uiProgress" />
        <TextBlock x:Name="uiStatus" />
[[XAML+SERVICE+LIST]]
        <Button Content="REREAD" Click="OnRereadDevice" />
    </StackPanel>
</Page>
```



## XAML+CHARACTERISTIC+DATA+PROPERTY Type=list Source=Services/Characteristics/Properties ListOutput=parent

Feel free to add in this little explainer to the code
                    <!-- Data=[[DataName]] Characteristic=[[CharacteristicName]] Service=[[ServiceName]] -->

```
                    <TextBox IsReadOnly="[[IS+READ+ONLY]]" x:Name="[[CharacteristicName.dotNet]]_[[DataName.dotNet]]" Text="[[DEFAULT+VALUE]]" Header="[[DATANAMEUSER]]" Style="{StaticResource HEXStyle}"/>
```

## XAML+READWRITE+BUTTON+LIST Type=list Source=Services/Characteristics/Buttons ListOutput=parent CodeListSubZero=""

```
                    <Button Content="[[ButtonVerb]]" Click="On[[ButtonVerb]][[CharacteristicName.dotNet]]" />
```

## XAML+CHART Type=list Source=Services/Characteristics ListOutput=child

```
<charts:ChartControl Height="200" Width="500" x:Name="[[CharacteristicName.dotNet]]Chart" />
```

## XAML+ENUM+SIMPLE+BUTTON+LIST If="[[ButtonSource]] == Enums" Type=list Source=Services/Characteristics/Enums ListOutput=parent 

Was PageXamlCharacteristicEnumButtonTemplate. 
Creates simple buttons from a list of Enums; this makes for pretty seriously ugly buttons.
The preferred approach is to make a buttonUI item with a Buttons list (see the William Weiler Skoobot)

```
                    <Button Content="[[EnumName]]" Tag="[[EnumValue]]" Click="OnClick[[CharacteristicName.dotNet]]" />
```


## XAML+UI+BUTTON+LIST+CONTENT+BUTTON If="[[ButtonType]] contains Button" Type=list Source=Services/Characteristics/ButtonUI ListOutput=child  Trim=true

```
                    <Button Content="[[ButtonLabel]]" Tag="[[ButtonEnumValue]]" Click="OnClick[[CharacteristicName.dotNet]]" />
```
        
## XAML+UI+BUTTON+LIST+CONTENT+RECTANGLE If="[[ButtonType]] contains Blank" Type=list Source=Services/Characteristics/ButtonUI ListOutput=child Trim=true

```
                    <Rectangle />
```
        

## XAML+UI+BUTTON+LIST If="[[ButtonSource]] contains ButtonUI" Type=list Source=Services/Characteristics/ButtonUI ListOutput=parent 

TODO: this is the new list I'm working on here!here
It is like the XAML+ENUM+SIMPLE+BUTTON+LIST but creates much nicer buttons

```
[[XAML+UI+BUTTON+LIST+CONTENT+BUTTON]][[XAML+UI+BUTTON+LIST+CONTENT+RECTANGLE]]
```


        
## XAML+ENUM+BUTTON+LIST+PANEL If="[[ButtonSource]] !contains None" Type=list Source=Services/Characteristics ListOutput=child
Was PageXamlCharacteristicEnumButtonPanelTemplate


```
                <VariableSizedWrapGrid Orientation="Horizontal" MaximumRowsOrColumns="[[buttonMaxColumns]]">
[[XAML+UI+BUTTON+LIST]][[XAML+ENUM+SIMPLE+BUTTON+LIST]]
                </VariableSizedWrapGrid>	
```



## XAML+UILIST+LIST+CONTENT+BLANK If="[[UIType]] contains Blank" Type=list Source=Services/Characteristics/UIList ListOutput=child  Trim=true

```
                    <Rectangle />
```
## XAML+UILIST+LIST+CONTENT+BUTTON If="[[UIType]] contains ButtonFor" Type=list Source=Services/Characteristics/UIList ListOutput=child  Trim=true

```
                    <Button Content="[[Label]]" Click="[[FunctionName]]_ButtonClick" />
```


## XAML+UILIST+LIST+CONTENT+COMBOBOX+LIST If="[[UIType]] contains ComboBoxFor" Type=list Source=Services/Characteristics/UIList/RadioFor ListOutput=parent

The ComboBoxFor and RadioFor share the same child list called "RadioFor"

NOTE: Cannot set the Trim=true because it cuts the list short? That makes no sense. For Elegoo MiniCar.

```
                            <ComboBoxItem Content="[[RadioName]]" Tag="[[RadioValue]]" />
```

## XAML+UILIST+LIST+CONTENT+COMBOBOX+CONTROL If="[[UIType]] contains ComboBoxFor" Type=list Source=Services/Characteristics/UIList ListOutput=child  Trim=true

```
                    <ComboBox Header="[[Sub_Label]]" SelectedIndex="0" MinWidth="140" SelectionChanged="[[FunctionName]]_ComboBoxChanged">
[[XAML+UILIST+LIST+CONTENT+COMBOBOX+LIST]]
                    </ComboBox>
```



## XAML+UILIST+LIST+CONTENT+RADIO+LIST If="[[UIType]] contains RadioFor" Type=list Source=Services/Characteristics/UIList/RadioFor ListOutput=parent

NOTE: Cannot set the Trim=true because it cuts the list short? That makes no sense. For Elegoo MiniCar.

```
                            <RadioButton Content="[[RadioName]]" IsChecked="[[RadioChecked]]" Tag="[[RadioValue]]" Checked="[[FunctionName]]_RadioCheck"  />
```

## XAML+UILIST+LIST+CONTENT+RADIO+PANEL If="[[UIType]] contains RadioFor" Type=list Source=Services/Characteristics/UIList ListOutput=child  Trim=true

```
                    <StackPanel>
[[XAML+UILIST+LIST+CONTENT+RADIO+LIST]]
                    </StackPanel>
```

## XAML+UILIST+LIST+CONTENT+ROWSTART If="[[UIType]] contains RowStart" Type=list Source=Services/Characteristics/UIList ListOutput=child  Trim=true

```
                    <VariableSizedWrapGrid Orientation="Horizontal" MaximumRowsOrColumns="[[N]]">
```

## XAML+UILIST+LIST+CONTENT+ROWEND If="[[UIType]] contains RowEnd" Type=list Source=Services/Characteristics/UIList ListOutput=child  Trim=true

```
                    </VariableSizedWrapGrid>
```

## XAML+UILIST+LIST+CONTENT+SLIDER If="[[UIType]] contains SliderFor" Type=list Source=Services/Characteristics/UIList ListOutput=child  Trim=true

```
                    <Slider Header="[[Sub_Label]]" Value="[[Slider_Init]]" Minimum="[[Slider_Min]]" Maximum="[[Slider_Max]]" ValueChanged="[[FunctionName]]_SliderChanged" />
```

## XAML+UILIST+LIST If="[[UIListType]] !contains None" Type=list Source=Services/Characteristics/UIList ListOutput=parent 

```
[[XAML+UILIST+LIST+CONTENT+BLANK]][[XAML+UILIST+LIST+CONTENT+COMBOBOX+CONTROL]][[XAML+UILIST+LIST+CONTENT+BUTTON]][[XAML+UILIST+LIST+CONTENT+RADIO+PANEL]][[XAML+UILIST+LIST+CONTENT+ROWSTART]][[XAML+UILIST+LIST+CONTENT+ROWEND]][[XAML+UILIST+LIST+CONTENT+SLIDER]]
```

## XAML+UILIST+PANEL If="[[UIListType]] !contains None" Type=list Source=Services/Characteristics ListOutput=child
Was PageXamlCharacteristicEnumButtonPanelTemplate


```
                <StackPanel>
[[XAML+UILIST+LIST]]
                </StackPanel>	
```


## XAML+TABLE If="[[TableType]] contains standard" Type=list Source=Services/Characteristics ListOutput=child 

We only add in the XAML+TABLE if the characteristic.UI?.tableType is not null or empty (but it's never null). Valid tableType values are "standard" or nothing.

```
                    <controls:Expander Header="[[CharacteristicName]] Data tracker" IsExpanded="false" MinWidth="550" HorizontalAlignment="Left">
                        <StackPanel MinWidth="550">
                            [[XAML+CHART]]
                            <controls:DataGrid Style="{StaticResource TableStyle}" x:Name="[[CharacteristicName.dotNet]]Grid" ItemsSource="{Binding [[CharacteristicName.dotNet]]RecordData}" />
                            <TextBox  x:Name="[[CharacteristicName.dotNet]]_Notebox" KeyDown="On[[CharacteristicName.dotNet]]_NoteKeyDown" />
                            <StackPanel Orientation="Horizontal" HorizontalAlignment="Left">
                                <ComboBox SelectionChanged="OnKeepCount[[CharacteristicName.dotNet]]" Header="Keep how many items?" SelectedIndex="2">
                                    <ComboBoxItem Tag="10">10</ComboBoxItem>
                                    <ComboBoxItem Tag="100">100</ComboBoxItem>
                                    <ComboBoxItem Tag="1000">1,000</ComboBoxItem>
                                    <ComboBoxItem Tag="10000">10K</ComboBoxItem>
                                </ComboBox>
                                <Rectangle Width="5" />
                                <ComboBox SelectionChanged="OnAlgorithm[[CharacteristicName.dotNet]]" Header="Remove algorithm?" SelectedIndex="0">
                                    <ComboBoxItem Tag="1" ToolTipService.ToolTip="Keep a random sample of data">Keep random sample</ComboBoxItem>
                                    <ComboBoxItem Tag="0" ToolTipService.ToolTip="Keep the most recent data">Keep latest data</ComboBoxItem>
                                </ComboBox>
                                <Button Content = "Copy" Click="OnCopy[[CharacteristicName.dotNet]]" />
                            </StackPanel>
                        </StackPanel>
                    </controls:Expander>
```

## XAML+CHARACTERISTIC+LIST Type=list Source=Services/Characteristics ListOutput=parent

TODO: the original code would add a </stackpanel><stackpanel> every 5 items. That keeps everything a bit 
more tidy. However, that's not possible with the new system.

Instead, the code should be updated to be a GridView where the ItemsPanel is set to be an ItemsWrapGrid
            <GridView.ItemsPanel>
                <ItemsPanelTemplate>
                    <ItemsWrapGrid MaximumRowsOrColumns="3"/>
                </ItemsPanelTemplate>
            </GridView.ItemsPanel>

```
                <TextBlock Style="{StaticResource SubheaderStyle}">[[CharacteristicName]]</TextBlock>
            <GridView>
            <GridView.ItemsPanel>
                <ItemsPanelTemplate>
                    <ItemsWrapGrid Orientation="Horizontal" MaximumRowsOrColumns="5"/>
                </ItemsPanelTemplate>
            </GridView.ItemsPanel>
[[XAML+CHARACTERISTIC+DATA+PROPERTY]]
[[XAML+READWRITE+BUTTON+LIST]]
                </GridView>
[[XAML+ENUM+BUTTON+LIST+PANEL]]
[[XAML+FUNCTIONUI+LIST+PANEL]]
[[XAML+UILIST+PANEL]]
[[XAML+TABLE]]
```


## XAML+SERVICE+LIST Type=list Source=ServicesByPriority

TODO: investigate if this should be ListOutput=parent -- it seems like almost all Type=list should be a ListOutput=parent 

```
        <!-- XAML+SERVICE+LIST for [[ServiceName]] -->
        <controls:Expander Header="[[ServiceName]]" IsExpanded="[[ServiceIsExpanded]]" Style="{StaticResource HeaderStyleExpander}">
            <StackPanel Style="{StaticResource ChacteristicListStyle}">
            [[XAML+CHARACTERISTIC+LIST]]
            </StackPanel>
        </controls:Expander>
```






## XAML+FUNCTIONUI+LIST+PANEL

PageXamlFunctionUIListPanelTemplate 
The FunctionUIList puts in buttons, radio, slider etc. based on the characteristic "UIList" which is only part
of the Elegoo mini car.

The section is empty until it is truly needed.

```
```

Original code section (non-functional)
[[TAB]]<StackPanel>
[[FUNCTIONUILIST]]
[[TAB]]</StackPanel>
end to be replaced

## PageXamlFunctionComboBoxTemplate 
```
[[TAB]]<ComboBox Header="[[LABEL]]" [[COMBOINIT]] MinWidth="140" SelectionChanged="[[COMMAND]]_[[PARAM]]_ComboBoxChanged">
[[COMBOBOXLIST]]
[[TAB]]</ComboBox>
```


## PageXamlFunctionButtonTemplate 
```
[[TAB]]<Button Content="[[LABEL]]" Click="[[FUNCTIONNAME]]_ButtonClick" />
```
## PageXamlFunctionSliderTemplate
```
[[TAB]]<Slider Header="[[LABEL]]" Value="[[INIT]]" Minimum="[[MIN]]" Maximum="[[MAX]]" ValueChanged="[[COMMAND]]_[[PARAM]]_SliderChanged" />
```

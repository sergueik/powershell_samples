# origin: http://mark-dot-net.blogspot.com/2007/07/creating-custom-wpf-button-template-in.html
#requires -version 2
Add-Type -AssemblyName PresentationFramework
[xml]$xaml =
@"
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
xmlns:sys="clr-namespace:System;assembly=mscorlib" 
xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

<Window.Resources>
    <Style x:Key="MyFocusVisual">
      <Setter Property="Control.Template">
        <Setter.Value>
          <ControlTemplate TargetType="{x:Type Control}">
              <Grid Margin="3 2">
                <Rectangle Name="r1" StrokeThickness="1" Stroke="Black" StrokeDashArray="2 2"/>
                <Border Name="border" Width="{TemplateBinding ActualWidth}" Height="{TemplateBinding ActualHeight}"  CornerRadius="2" BorderThickness="1" />
              </Grid>
          </ControlTemplate>
        </Setter.Value>
      </Setter>
    </Style>

    <Style x:Key="ShadowStyle">
    	<Setter Property="Control.Foreground" Value="LightGray" />
    </Style>

    <Style x:Key="InformButton" TargetType="Button">
        <Setter Property="OverridesDefaultStyle" Value="True"/>
        <Setter Property="Margin" Value="2"/>
        <Setter Property="FontFamily" Value="Verdana"/>
        <Setter Property="FontSize" Value="11px"/>
        <Setter Property="FontWeight" Value="Bold"/>
        <Setter Property="FocusVisualStyle" Value="{StaticResource MyFocusVisual}" />
        <Setter Property="Background" >
        	<Setter.Value>
        		<LinearGradientBrush StartPoint="0,0" EndPoint="0,1" >
        			<GradientStop Color="#FFFFD190" Offset="0.2"/>
        			<GradientStop Color="Orange" Offset="0.85"/>
        			<GradientStop Color="#FFFFD190" Offset="1"/>
        		</LinearGradientBrush>
        	</Setter.Value>
        </Setter>
        <Setter Property="Template">
        	<Setter.Value>
        		<ControlTemplate TargetType="Button">
        			<Border Name="border" 
        				BorderThickness="1"
        				Padding="4,2" 
        				BorderBrush="DarkGray" 
        				CornerRadius="3" 
        				Background="{TemplateBinding Background}">
        				<Grid >
        				<ContentPresenter HorizontalAlignment="Center" 
        	                           VerticalAlignment="Center" Name="contentShadow" 
        					Style="{StaticResource ShadowStyle}">
        					<ContentPresenter.RenderTransform>
        						<TranslateTransform X="1.0" Y="1.0" />
        					</ContentPresenter.RenderTransform>
        				</ContentPresenter>
        				<ContentPresenter HorizontalAlignment="Center" 
                                    VerticalAlignment="Center" Name="content"/>
        				</Grid>
        			</Border>
        			<ControlTemplate.Triggers>
        				<Trigger Property="IsMouseOver" Value="True">
        					<Setter TargetName="border" Property="BorderBrush" Value="#FF4788c8" />
        					<Setter Property="Foreground" Value="#FF4788c8" />
        				</Trigger>
        				<Trigger Property="IsPressed" Value="True">					
        					<Setter Property="Background" >
        					<Setter.Value>
        						<LinearGradientBrush StartPoint="0,0" EndPoint="0,1" >
        							<GradientStop Color="#FFFFD190" Offset="0.35"/>
        							<GradientStop Color="Orange" Offset="0.95"/>
        							<GradientStop Color="#FFFFD190" Offset="1"/>
        						</LinearGradientBrush>
        					</Setter.Value>
        					</Setter>
        					<Setter TargetName="content" Property="RenderTransform" >
        					<Setter.Value>
        						<TranslateTransform Y="1.0" />
        					</Setter.Value>
        					</Setter>
        				</Trigger>
        				<Trigger Property="IsDefaulted" Value="True">
        					<Setter TargetName="border" Property="BorderBrush" Value="#FF282828" />
        				</Trigger>
        				<Trigger Property="IsFocused" Value="True">
        					<Setter TargetName="border" Property="BorderBrush" Value="#FF282828" />
        				</Trigger>
        				<Trigger Property="IsEnabled" Value="False">
        					<Setter TargetName="border" Property="Opacity" Value="0.7" />
        					<Setter Property="Foreground" Value="Gray" />
        				</Trigger>
        
        			</ControlTemplate.Triggers>
        		</ControlTemplate>
        	</Setter.Value>
        </Setter>
    </Style>
</Window.Resources>

<StackPanel HorizontalAlignment="Center">
    <Button Style="{StaticResource InformButton}">Hello</Button>
    <Button Style="{StaticResource InformButton}">World</Button>
    <Button Style="{StaticResource InformButton}" FontSize="20">Big Button</Button>
    <Button Name="button1" Style="{StaticResource InformButton}" IsDefault="True">Default</Button>
    <Button Style="{StaticResource InformButton}" IsEnabled="False">Disabled</Button>
    <Button Style="{StaticResource InformButton}" Width="70" Height="30">70 x 30</Button>
    <TextBox />
    <Button Style="{StaticResource InformButton}" Width="30" Height="30">
        <Path Fill="Black" Data="M 3,3 l 9,9 l -9,9 Z" />
    </Button>
</StackPanel>

</Window>
"@

Clear-Host
$reader = (New-Object System.Xml.XmlNodeReader $xaml)
$target = [Windows.Markup.XamlReader]::Load($reader)
$control = $target.FindName("button1")
$eventMethod = $control.add_click
$eventMethod.Invoke({ $target.Title = "Hello $((Get-Date).ToString('G'))" })
$target.ShowDialog() | Out-Null

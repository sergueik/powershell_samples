// origin: https://www.cyberforum.ru/vbscript-wsh/thread2723876.html
// does not appear to work
// see also: https://www.developer.com/microsoft/dotnet/creating-a-app-using-jscript-net-and-windows-forms/	
function vote_form()
{
 
  this.form = WScript.CreateObject("System.Windows.Forms.Form");
  this.radioButton1 = WScript.CreateObject("System.Windows.Forms.RadioButton");
  this.radioButton2 = WScript.CreateObject("System.Windows.Forms.RadioButton");
  this.radioButton3 = WScript.CreateObject("System.Windows.Forms.RadioButton");
  this.radioButton4 = WScript.CreateObject("System.Windows.Forms.RadioButton");
  this.button1 = WScript.CreateObject("System.Windows.Forms.Button");
  this.button2 = WScript.CreateObject("System.Windows.Forms.Button");
  this.linkLabel1 = WScript.CreateObject("System.Windows.Forms.LinkLabel");
  
  //Настраиваем контролы
  with(this.radioButton1)
  {
    Parent = this.form;
    Checked = true;
    Left = 12;
    Top = 12;
    Width = 110;
    Height = 17;
    TabStop = true;
    Text = "VBScript";
  }
 
  with(this.radioButton2)
  {
    Parent = this.form;
    Left = 12;
    Top = 35;
    Width = 110;
    Height = 17;
    TabStop = true;
    Text = "JScript";
  }
 
  with(this.radioButton3)
  {
    Parent = this.form;
    Left = 12;
    Top = 58;
    Width = 110;
    Height = 17;
    TabStop = true;
    Text = "PerlScript";
  }
 
  with(this.radioButton4)
  {
    Parent = this.form;
    Left = 12;
    Top = 81;
    Width = 110;
    Height = 17;
    TabStop = true;
    Text = "Единая Россия";
  }
  
  with(this.button1)
  {
    Parent = this.form;
    Left = 12;
    Top = 112;
    Width = 85;
    Height = 23;
    Text = "Да!";
    DialogResult = 1;
  }
  
  with(this.button2)
  {
    Parent = this.form;
    Left = 125;
    Top = 112;
    Width = 85;
    Height = 23;
    Text = "Идите вы!";
    DialogResult = 0;
  }
  
  with(this.linkLabel1)
  {
    Parent = this.form;
    Left = 167;
    Top = 9;
    Width = 45;
    Height = 15;
    Text = "kaimi.ru";
  }
  
  //настраиваем форму
  with(this.form)
  {
    Width = 222;
    Height = 125;
    Text = "Какой язык вам больше по душе?";
    AutoSize = true;
    FormBorderStyle = 5; //FixedToolWindow
    CancelButton = this.button1;
    CancelButton = this.button2;
  }
var default_color;
function mouseIn() {
  default_color = document.changecolorbutton.but.style.background;
  document.changecolorbutton.but.style.background = "red";
}
function mouseOut() {
  document.changecolorbutton.but.style.background = default_color;
}
  
  //Отобразить форму и вернуть true, если пользователь нажал на первую кнопку
  this.show = function()
  {
    this.form.ShowDialog();
    return this.form.DialogResult == 1;
  };
  
  //Получить выбранный результат (см. выше, на форме 4 radio button'а)
  this.result = function()
  {
    if(this.radioButton1.Checked)
      return this.radioButton1.Text;
    else if(this.radioButton2.Checked)
      return this.radioButton2.Text;
    else if(this.radioButton3.Checked)
      return this.radioButton3.Text;
    else if(this.radioButton4.Checked)
      return this.radioButton4.Text;
  };
};
 
//Создаем форму
var my_form = new vote_form;
 
//Предлагаем пользователю сделать выбор
while(true)
{
  if(my_form.show())
  {
    WScript.Echo("Вы выбрали: " + my_form.result());
    break;
  }
  else
  {
    WScript.Echo("Ну как же так, надо же выбрать!");
  }
}

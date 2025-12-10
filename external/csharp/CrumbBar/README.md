### Info

C # __CrumbBar class__

![screenshot](https://github.com/sergueik/powershell_samples/blob/master/external/csharp/CrumbBar/screenshots/screenshot.jpg)
### Usage
* initialiation
```c#
crumbBar = new CrumbBar();
crumbBar.Location = new Point(10, 10);
crumbBar.Font = new Font("Segoe UI", 9);
crumbBar.ForeColor = Color.Black;
// crumbBar.BackColor = Color.Transparent; // optional

crumbBar.Size = new Size(560, 24);
AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
ClientSize = new System.Drawing.Size(1113, 615);
```
* Add some crumbs one at a time
```
crumbBar.Add("Home");
crumbBar.Add("Documents");
```
* Add `IEnumerable` data in one call: 
```c#

crumbBar.addRange( new List<string> { "Home", "Documents", "Projects" } );
```
add path-like Strings
```c#
crumbBar.AddPath("https://example.com/home/products/widgets", '/');
```

### Author

[Serguei Kouzmine](mailto:kouzmine_serguei@yahoo.com)

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAntiforgery(options =>
                                    {
                                        options.Cookie.Name = "XSRF-TOKEN";
                                        options.HeaderName = "X-XSRF-TOKEN";
                                    })
                .AddControllersWithViews();

// Per accedere ai coocky
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

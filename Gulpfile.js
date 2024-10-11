/// <binding Clean='clean' />
"use strict";

let gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    rename = require("gulp-rename"),
    deletefile = require("gulp-delete-file");

let paths = {
    webroot: "./wwwroot/",
    bootstrap: "./wwwroot/lib/bootstrap/dist/",
    jquery: "./wwwroot/lib/jquery/dist/"
};

paths.js = paths.webroot + "lib/custom/js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";

paths.css = paths.webroot + "lib/custom/css/**/*.css";
paths.minCss = paths.webroot + "css/custom/**/*.min.css";

paths.concatJs = paths.webroot + "js/**.js";
paths.concatCss = paths.webroot + "css/*.min.css";

paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";

paths.bootstrapJs = paths.bootstrap + "js/**/*.js";
paths.bootstrapCss = paths.bootstrap + "css/**/*.css";

paths.jqueryJs = paths.jquery + "js/**/*.js";

// Clean js folder
gulp.task("clean:js", async function (cb) {
    rimraf(paths.concatJs, cb);
});

// Clean css folder
gulp.task("clean:css", async function (cb) {
    rimraf(paths.concatCss, cb);
});

// Clean totale
gulp.task("Clean", gulp.series("clean:js", "clean:css"));

// Minifica tutti i .js del progetto. Poi li raccoglie nella cartella js. Facendo un solo wrapper non funziona
gulp.task("min:js", async function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

// Minifica tutti i .css del progetto e li raggruppa in site.min.css
gulp.task("min:css", async function () {
    return gulp.src([paths.css, "!" + paths.minCss], { base: "." })
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

// Costruisce la js folder
gulp.task('lib:js', async function () {
    return gulp.src([
        './node_modules/bootstrap/dist/js/bootstrap.min.js',
        './node_modules/jquery-ui/ui/widgets/datepicker.js',
        './node_modules/jquery-ui/ui/i18n/datepicker-en-GB.js',
        './node_modules/jquery-ui/ui/i18n/datepicker-it.js',
        './node_modules/@popperjs/core/dist/umd/popper.js',
        './node_modules/jquery/dist/jquery.min.js',
        './node_modules/jquery-ui/dist/jquery-ui.min.js',
    ])
        .pipe(gulp.dest(paths.webroot + 'js'));
});

// Rename
gulp.task('rename:en-GB', async function () {
    let regexp = /\.js$/;
    gulp.src(paths.webroot + "js/datepicker-en-GB.js")
        .pipe(rename(function (path) {
            // Returns a completely new object, make sure you return all keys needed!
            return {
                dirname: path.dirname,
                basename: "datepicker-en",
                extname: ".js"
            };
        }))
        .pipe(gulp.dest(paths.webroot + "js"));
    gulp.src([paths.webroot + "js/datepicker-en-GB.js"])
        .pipe(deletefile({
            reg: regexp,
            deleteMatch: true
        }));
});

// Costruisce la css folder
gulp.task('lib:css', async function () {
    return gulp.src([
        './node_modules/bootstrap/dist/css/bootstrap.min.css',
        './node_modules/jquery-ui/dist/themes/base/jquery-ui.min.css',
    ])
        .pipe(gulp.dest(paths.webroot + 'css'));
});

// Costruzione totale (eccetto le ommagini)
gulp.task("Add", gulp.series(['min:js', 'min:css', 'lib:js', 'lib:css']));

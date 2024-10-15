"use strict";

const gulp = require("gulp"),
      concat = require("gulp-concat"),
      cssmin = require("gulp-cssmin"),
      uglify = require("gulp-uglify"),
      rename = require("gulp-rename"),
      deletefile = require("gulp-delete-file"),
      image = require("gulp-imagemin");

const paths = {
      webroot: "./wwwroot/",
      node_modules: "./node_modules/"
};

const regexp = /\.(css|map|png|js)$/;

paths.JsLib = paths.webroot + "lib/custom/js/";
paths.CssLib = paths.webroot + "lib/custom/css/";

paths.JsDest = paths.webroot + "js/";
paths.CssDest = paths.webroot + "css/";
paths.CssImagesDest = paths.webroot + "css/images/";

gulp.task("clean", async () => {
    return await gulp.src([paths.CssDest + '**/*', paths.JsDest + "**/*" ])
        .pipe(deletefile({
            reg: regexp,
            deleteMatch: true
        }));
});

gulp.task("copy", async () => {
    gulp.src([ paths.JsLib + "**/*.js" ], { base: "." })
        .pipe(concat(paths.JsDest + 'site.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest("."))
    gulp.src([paths.CssLib + '**/*.css' ], { base: "." })
        .pipe(concat(paths.CssDest + 'site.min.css'))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
    gulp.src([
        paths.node_modules + 'bootstrap/dist/css/bootstrap.min.css',
        paths.node_modules + 'bootstrap/dist/css/bootstrap.min.css.map',
        paths.node_modules + 'jquery-ui/dist/themes/base/jquery-ui.min.css',
    ])
        .pipe(gulp.dest(paths.CssDest));
    gulp.src([
        paths.node_modules + 'jquery-ui/dist/themes/base/images/*.*',
    ])
        .pipe(image())
        .pipe(gulp.dest(paths.CssImagesDest));
    return await gulp.src([
        paths.node_modules + 'bootstrap/dist/js/bootstrap.min.js',
        paths.node_modules + 'jquery-ui/ui/widgets/datepicker.js',
        paths.node_modules + 'jquery-ui/ui/i18n/datepicker-en-GB.js',
        paths.node_modules + 'jquery-ui/ui/i18n/datepicker-it.js',
        paths.node_modules + '@popperjs/core/dist/umd/popper.js',
        paths.node_modules + 'jquery/dist/jquery.min.js',
        paths.node_modules + 'jquery-ui/dist/jquery-ui.min.js',
    ])
        .pipe(gulp.dest(paths.JsDest)).on('end', () => { console.log('fatto'); });
});

gulp.task('ren:en-GB', async () => {
    return await gulp.src(paths.JsDest + "datepicker-en-GB.js")
        .pipe(rename(function (path) {
            return {
                dirname: path.dirname,
                basename: "datepicker-en",
                extname: ".js"
            };
        }))
        .pipe(gulp.dest(paths.JsDest));
});

gulp.task('del:en-GB', async () => {
    return await gulp.src([ paths.JsDest + "datepicker-en-GB.js" ])
        .pipe(deletefile({
            reg: regexp,
            deleteMatch: true
        }));
});

gulp.task('build', gulp.series('copy', 'ren:en-GB'));
gulp.task('rebuild', gulp.series('clean', 'copy', 'ren:en-GB', 'del:en-GB'));
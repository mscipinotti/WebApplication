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

const regexp = /\.(css|map|png|svg|jpg|ico|js)$/;

paths.JsLib = paths.webroot + "lib/custom/js/";
paths.CssLib = paths.webroot + "lib/custom/css/";
paths.ImageLib = paths.webroot + "lib/images/";

paths.JsDest = paths.webroot + "js/";
paths.CssDest = paths.webroot + "css/";
paths.CssImagesDest = paths.webroot + "css/images/";
paths.ImageDest = paths.webroot + "images/";
paths.ImageIconsDest = paths.webroot + "images/favicons";

gulp.task("clean", async () => {
    return await gulp.src([paths.CssDest + '**/*', paths.CssImagesDest + '**/*', paths.JsDest + "**/*", paths.ImageDest + '**/*', + paths.ImageIconsDest + '**/*'])
        .pipe(deletefile({
            reg: regexp,
            deleteMatch: true
        }));
});

gulp.task("copy", async (callback) => {
    gulp.src([paths.JsLib + "**/*.js"], { base: "." })
        .pipe(concat(paths.JsDest + 'site.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest("."))
    gulp.src([paths.CssLib + '**/*.css'], { base: "." })
        .pipe(concat(paths.CssDest + 'site.min.css'))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
    gulp.src([
            paths.node_modules + 'bootstrap/dist/css/bootstrap.min.css',
            paths.node_modules + 'bootstrap/dist/css/bootstrap.min.css.map',
            paths.node_modules + 'jquery-ui/dist/themes/base/jquery-ui.min.css',
        ]).pipe(gulp.dest(paths.CssDest));
    gulp.src([
            paths.node_modules + 'jquery-ui/dist/themes/base/images/*.*',
        ]).pipe(image())
          .pipe(gulp.dest(paths.CssImagesDest));
    gulp.src([
        paths.ImageLib + '*.*',
        ]).pipe(image())
          .pipe(gulp.dest(paths.ImageDest));
    gulp.src([
        paths.ImageLib + 'favicons/*.*',
        ]).pipe(image())
          .pipe(gulp.dest(paths.ImageIconsDest));

    // gulp 4.0 e successivi serializza la promise non lo stream per cui il task 'rebuild' se non si ritorna una Promise non funziona. Lo stream assicura che i file siano stati creati.
    // in questa serie di file c'è datepicker-en-GB.js che deve essere rinominato (viene creato uno uguale con nome corretto e poi si cancella l'originale).
    await promisifyStream(
        gulp.src([
            paths.node_modules + 'bootstrap/dist/js/bootstrap.min.js',
            paths.node_modules + 'jquery-ui/ui/widgets/datepicker.js',
            paths.node_modules + 'jquery-ui/ui/i18n/datepicker-en-GB.js',
            paths.node_modules + 'jquery-ui/ui/i18n/datepicker-it.js',
            paths.node_modules + '@popperjs/core/dist/umd/popper.js',
            paths.node_modules + 'jquery/dist/jquery.min.js',
            paths.node_modules + 'jquery-ui/dist/jquery-ui.min.js',
            ]).pipe(gulp.dest(paths.JsDest))
        );
});

function promisifyStream(stream) {
    return new Promise(res => stream.on('end', res));
}

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
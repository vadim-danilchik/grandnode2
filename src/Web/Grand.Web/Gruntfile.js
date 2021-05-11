/// <binding ProjectOpened='watch:tasks' />
/*
This file in the main entry point for defining grunt tasks and using grunt plugins.
Click here to learn more. https://go.microsoft.com/fwlink/?LinkID=513275&clcid=0x409
*/
module.exports = function (grunt) {
    grunt.initConfig({
        clean: ["wwwroot/bundle/*"],
        concat: {
            js: {
                src: ['wwwroot/theme/lib/vue.min.js',
                    'wwwroot/theme/lib/portal-vue.umd.min.js',
                    'wwwroot/theme/lib/bootstrap-vue.min.js',
                    'wwwroot/theme/lib/bootstrap-vue-icons-slim.min.js',
                    'wwwroot/theme/lib/axios.min.js',
                    'wwwroot/theme/lib/vue-awesome-countdown.umd.min.js',
                    'wwwroot/theme/lib/vee-validate.minimal.min.js',
                    'wwwroot/theme/script/validation-config.min.js'],
                dest: 'wwwroot/bundle/bundle.js'
            },
            css: {
                src: ['wwwroot/theme/bootstrap-vue/bootstrap.min.css',
                    'wwwroot/theme/bootstrap-vue/bootstrap-vue.min.css'],
                dest: 'wwwroot/bundle/bundle.min.css'
            },            
        },
        uglify: {
            js: {
                src: ['wwwroot/bundle/bundle.js'],
                dest: 'wwwroot/bundle/bundle.min.js'
            }
        },
        cssmin: {
            target: {
                src: ["wwwroot/theme/bootstrap-vue/bootstrap.min.css", "wwwroot/theme/bootstrap-vue/bootstrap-vue.min.css"],
                dest: "wwwroot/bundle/bundle.min.css"
            }
        }
    });
    grunt.loadNpmTasks("grunt-contrib-clean");
    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-contrib-cssmin');

    grunt.registerTask("default", ["cssmin"]);

};
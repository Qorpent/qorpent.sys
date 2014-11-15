= Qorpent HOST Web Module Template

.dir in most folders - method to keep project structure with empty folders in git

package.json		-- node.js configuration and defaults for optimize.js, build.bsproj
make.sh			    -- shell runner for targets
bld				    -- sh shortcut for build.sh build all
bld.cmd			    -- cmd shortcut for build.sh build all

/dist               -- distribution module files
  |_ /css 			-- CSS's folder for production
  |_ /fonts			-- font's assets folder
  |_ /img 		    -- Image's assets folder, including Compass sprite generated files
  |_ /js			-- JS production folder (minimized versions)
  |_ /views			-- ANGULAR views folder
  |_ /wiki          -- distribution level WIKI

/src                -- directory for sources to be just raw for compilation
  |_ /js			-- JS/TypeScript source folder (source versions of JS and TypeScript)
  |_ /bxls			-- BXLS source files
  |_ /cs			-- C# files
    |_ /auto        -- auto generated C# from B#
  |_ /css		    -- CSS/SCSS folder for  generation

/tests				-- JS/css testing folder

/lib 				-- place external/system JS/css here
  |_ *.*			-- default set of modules  - require, mocha/chai, angular, required almost for any web module

/demo				-- samples, demo pages place here

/targets			-- build support files to call from make.sh
  |_ build			-- sh script to run all(default),bsc,compass,optimize
  |_ clean			-- removes all auto-generated and temporary files
  |_ bsc            -- BSC compiler runner
  |_ compass        -- COMPASS compiler runner
  |_ optimize       -- RequireJS optimizer runner
  |_ tsc            -- TypeScript compiler runner
  |_ bsc.bsproj		-- project file for B# part of module
  |_ compass.rb 	-- compass configuration
  |_ optimize.js	-- node.js requirejs optimization config (uses package.json if compile set up there or main.js)
  |_ main.js 		-- special JS module to be compiled for minimization by default
  |_ test           -- launch tests
  |_ browser.html   -- page to test with browser
  |_ phantom.js     -- script to be call for testing in phantomjs
  |_ node.js        -- script to be call for testing in nodejs
  |_ testmap.js     -- file to map tests (uses package.json by default)
  |_ browser.js     -- browser test main loader
  |_ phantom.sh     -- direct console call for testing in Phantom.JS
  |_ install-deps   -- first-time call script to setup node/ruby env for module

.gitignore			-- .gitignore file by default


= Web module references format in package.json
Define references to other modules in form "NAME" : "PATH TO JS"
Path types:
1) relative to ../src/js, starts with ., "./my/file", "../../my/file"
2) full paths, starts with "/" or "[DISC]:/", for ex "/usr/my", "c:/tmp/my"
3) std webmodule/[dist|src]/js/[name] path where webmodule started from repos,
   webmodule name used without '.webmodule' extension
   3.1) path to certain file in dist with ! - Qorpent.System/the!the-full
   3.2) path to certain file in src with ? - Qorpent.System/the?the-object
   3.3) default module - Qorpent.System/the == Qorpent.System/the!the

= Test registry format in package.json
1) simple string - means as ../tests/[name].js module without additional data provided
2) object, that can contains :
  2.1) "name" - name of test module (if not provided only "deps" part will be used)
  2.2) "condition" - "browser","phantom","node","!browser","!phantom","!node" or regex to match against context (href in browser or argv in node)
  2.3) "deps" - string array - if not full path provided - testmap will try guess path based on browser/node profile and webModuleDependency

= Compile setup with package.json support
1) "type" (optional, default "lib") - "lib" or "api" - "lib" describes a root "namespace" with other modules as it's members, if "root" provided,
   it's used as root object otherwise {}, "api" is always proxy for root and requires it , assumed that root completly describes module
2) "root" - required for "api", both "api" or "lib" if set - will be returned as root object
3) "include" - describes all modules (other to root) to be included in compilation (except of webModulesDependency)

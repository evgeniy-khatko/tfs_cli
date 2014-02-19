tfs_cli
=======

## Command line utility for following tasks:
### General usage:
- get all testcases from specific testplan on a TFS server to local XML file (including testcase attributes, steps and expected actions)
- update single testcase with provided results to a TFS testplan (can set different test and run results)

### TFS Cucumber integration:
- get all testcases from specific testplan on a TFS server to local Filesystem, in Cucumber format (.feature)
- update whole TFS testplan using provided Cucumber-JVM Junit report with following mapping: 
  Feature name => TFS testsuite
  Feature scenario => TFS testcase
  Scenario step => TFS testcase step
  (additianally TFS testcase results updated with comments and attachments)


## Run examples:
Note: fill in tfs_cli.exe.config with correct parameters beforehand

tfs_cli.exe get /o=tests.xml - get tests to XML
tfs_cli.exe features /o="C:\Users\UserName\Features" - get tests to .feature files
tfs_cli.exe update /rt="my test run" /tn="plancase" /to=Passed /duration=1000 - update single testcase
tfs_cli.exe junit /r="C:\path\to\TESTS-TestSuites.xml" - update testplan from junit

## Usage scenarios
- TFS integration with 3party test automation tools
- TFS integration with Cucumber-JVM

## Build instructions
- Open project in VS (developed with VS2012)
- Add "assembly" dlls to project
- Build


PROJECT = File.expand_path(File.dirname(__FILE__) + '/..')
MSPEC_PATH = File.join(PROJECT, 'depends/Machine.Specifications/mspec.exe')
NUNIT_PATH = '/Library/Frameworks/Mono.framework/Versions/2.10.9/lib/mono/4.0/nunit-console.exe'
SPEC_PATH = File.join(PROJECT, 'magnetic.example.specs/bin/Debug/magnetic.example.specs.dll')
TEST_PATH = File.join(PROJECT, 'magnetic.example.tests/bin/Debug/magnetic.example.tests.dll')

task :default => ['spec', 'test']

desc "Run specs"
task :spec => :compile do
  system "mono --runtime=v4.0 #{MSPEC_PATH} #{SPEC_PATH}"  
end

desc "Run tests"
task :test => :compile do
  system "mono #{NUNIT_PATH} #{TEST_PATH} -nologo -xml=./magnetic.example.tests/test-results/results.xml"
end

desc "Compile project"
task :compile do
  system "xbuild #{PROJECT}/*.sln"
end

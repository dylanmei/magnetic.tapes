
PROJECT = File.expand_path(File.dirname(__FILE__) + '/..')
MSPEC_PATH = File.join(PROJECT, 'depends/Machine.Specifications/mspec.exe')
NUNIT_PATH = '/Library/Frameworks/Mono.framework/Versions/2.10.9/lib/mono/4.0/nunit-console.exe'
TEST_PATH = File.join(PROJECT, 'magnetic.tapes.tests/bin/Debug/magnetic.tapes.tests.dll')
EX_SPEC_PATH = File.join(PROJECT, 'magnetic.example.specs/bin/Debug/magnetic.example.specs.dll')
EX_TEST_PATH = File.join(PROJECT, 'magnetic.example.tests/bin/Debug/magnetic.example.tests.dll')

task :default => 'test'

task :test => :compile do
  system "mono #{NUNIT_PATH} #{TEST_PATH} -nologo -xml=./magnetic.tapes.tests/test-results/results.xml"
end

task :compile do
  system "xbuild #{PROJECT}/*.sln"
end

task :example => ['example:spec', 'example:test']
namespace :example do
  desc "Run example tests"
  task :test => :compile do
    system "mono #{NUNIT_PATH} #{EX_TEST_PATH} -nologo -xml=./magnetic.example.tests/test-results/results.xml"
  end

  desc "Run example specs"
  task :spec => :compile do
    system "mono --runtime=v4.0 #{MSPEC_PATH} #{EX_SPEC_PATH}"  
  end
end

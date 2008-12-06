# ****************************************************************************
#
# Copyright (c) Microsoft Corporation. 
#
# This source code is subject to terms and conditions of the Microsoft Public License. A 
# copy of the license can be found in the License.html file at the root of this distribution. If 
# you cannot locate the  Microsoft Public License, please send an email to 
# ironruby@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
# by the terms of the Microsoft Public License.
#
# You must not remove this notice, or any other, from this software.
#
#
# ****************************************************************************

trap("INT") {
    puts 
    puts "!!! Interrupt by the user !!!"
    exit(-1)
}

THIS_DIRECTORY = File.expand_path(File.dirname(__FILE__))
CURR_DIRECTORY = Dir.pwd

require 'common'
require 'benchmark'

$failures = 0

class TestListFile
    def TestListFile.load
        test_lst_file = File.join(THIS_DIRECTORY, "test.lst")
        lines = ''
        
        # filter empty line and comment line
        File.open(test_lst_file, 'r') do |f| 
            lines = f.readlines.grep(/^[^#\s]+/)
        end
        
        lines.collect do |l|
            parts = l.split
            raise "unexpected test entry: #{l}" if parts.length > 2
            TestListFile.new(*parts) 
        end 
    end 
    
    def initialize(file_name, driver_list='all')
        @file_name = file_name
        @driver_list = driver_list 
    end 
    
    def should_run_with?(driver)
        return true if @driver_list == "all"
        return false if @driver_list == "none"
        if @driver_list.include? '-'
            # you do not have to specify "wanted"
            @driver_list.include?(driver.name + "-") == false
        else 
            @driver_list.include? driver.name
        end
    end 
    
    def run_by(driver)
        if !should_run_with? driver
            print "s"
        elsif driver.run(@file_name, driver.logger.log_name) == 0
            print "."
        else
            $failures += 1
            print "x(#{File.basename(@file_name)})"
        end 
    end
    
    def run_skipped_by(driver)
        if !should_run_with? driver
            if driver.run(@file_name, driver.logger.log_name) == 0
                printf "p(%s)", File.basename(@file_name)
            else
                print "f"
            end 
        end
    end 
end 

# current options which are running inside SNAP:
#   -checkin
#   -fast

# drivers
applicable_drivers = []
ARGV.each do |option|
    case option
    when /all/
        applicable_drivers << Test::Iron_m2 << Test::Iron_m3 << Test::Iron_cc
    when /coreclr/
        applicable_drivers << Test::Iron_cc
    when /fast|interpret/
        applicable_drivers << Test::Iron_m1
    when /compile/
        applicable_drivers << Test::Iron_m2
    when /verify/
        applicable_drivers << Test::Iron_m3
    when /cruby/
        applicable_drivers << Test::CRuby
    when /checkin/
        applicable_drivers << Test::Iron_m2 << Test::Iron_m3 << Test::CRuby
    when /neg/
    else
        p "Invalid option: #{option}"
    end
end
applicable_drivers = [ Test::CRuby, Test::Iron_m1 ] if applicable_drivers.empty?
applicable_drivers.uniq!

test_files = TestListFile::load 

bms = []
applicable_drivers.each do |driver|
	bms << Benchmark.measure(driver.to_s) do
		puts "#{driver}"
		puts "    log @ #{driver.logger} \n"
	    
		if ARGV.include? "-neg"
			test_files.each { |tf| tf.run_skipped_by(driver) }
		else
			test_files.each { |tf| tf.run_by(driver) }
		end
		puts "\n\n"
   end		
end

bms.each { |t| puts t.format("%n\n%10.6r\n") }

if applicable_drivers.include? Test::Iron_m2 and !ARGV.include? "-fast"
    [
        'syntax',
        'compat'
    ].each do |s|
        puts ">>> Running #{s} test \n"
        ret = Test::CRuby.run(File.join(TestPath::TEST_DIR, "#{s}/run_#{s}.rb -snap "), nil)
        if ret == 0
            puts "pass"
        else
            $failures += ret
            puts "fail"
        end
        puts "\n"
    end
end 

puts "\nSummary: #{$failures}\n"

exit($failures)

#!/usr/bin/env python
from __future__ import print_function

from subprocess import check_call, check_output
import json
import shutil

check_call('brew install yarn', shell=True)
check_call('yarn install', shell=True)

package = json.loads(open('package.json').read())
package['name'] = raw_input('Project name: ')
package['description'] = raw_input('Project description: ')
package['repository'] = raw_input('Project repository: ')
with open('package.json', 'w') as f:
    f.write(json.dumps(package, indent=2))


shutil.move('GettingStarted.sln', package['name'] + '.sln')

print('Done! The project is now built.')
print('You can open public/index.html in your browser to run your code.')
print('You can use "yarn" to recompile.')
print('You can use "yarn watch" to automatically recompile when you save.')

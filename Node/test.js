const {add} = require('./myMath.js')

let num = 42;
var name = 'Tom';
let isStudent = true;

let color = ['red', 'green', 'blue']

let person = {name: 'Alice', age: 30}

console.log(add(num, num))

function greet(name)
{
    console.log('Hello ' + name + '!');
}

setTimeout(() => {
    console.log(1);
}, 1000);
setTimeout(() => {
    console.log(2);
}, 750);
setTimeout(() => {
    console.log(3);
}, 125);
setTimeout(() => {
    console.log(4);
}, 1200);
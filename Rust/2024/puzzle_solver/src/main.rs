use std::fs;
use regex::Regex;

// AdventOfCode 2024 - Day1 - Part 2

fn main() {
    let contents = fs::read_to_string("d:\\AdventOfCode2023\\InputFiles\\Day01.txt").unwrap();

    let mut left: Vec<i32> = Vec::new();
    let mut right: Vec<i32> = Vec::new();

    let re = Regex::new(r"\s*(?<left>\d+)\s+(?<right>\d+)").unwrap();

    //while(contents.len() > 0) {
    for (_, line) in contents.lines().enumerate() {

        if let Some(captures) = re.captures(line) {
            let l = captures.name("left").unwrap().as_str();
            let r = captures.name("right").unwrap().as_str();

            left.push(l.parse::<i32>().unwrap());
            right.push(r.parse::<i32>().unwrap());
        }
    }

    left.sort();
    right.sort();

    let mut sum = 0;

    for (_, &n) in left.iter().enumerate() {
        sum += n * right.iter().filter(|&&x| x == n).count() as i32;
    }

    println!("Puzzle solution: {}", sum);

}

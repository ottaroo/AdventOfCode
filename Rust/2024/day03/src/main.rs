use std::fs;
use regex::Regex;

fn main() {

    let corrupted_file = fs::read_to_string(r"d:\AdventOfCode2023\InputFiles\day03.txt").unwrap();
    let re = Regex::new(r"(?<do>don't\(\)|do\(\))|mul\((?<left>\d+),(?<right>\d+)\)").unwrap();

    let mut sum = 0;
    let mut do_multiplication = true;

    re.captures_iter(&corrupted_file).enumerate().for_each(|(_, c)| {

        if let Some(do_match) = c.name("do") {
            do_multiplication = do_match.as_str() == "do()";
            return;
        }

        if !do_multiplication {
            return;
        }

        let left: i32 = c.name("left").unwrap().as_str().parse().unwrap();
        let right: i32 = c.name("right").unwrap().as_str().parse().unwrap();

        sum += left * right;

    });

    println!("Puzzle solution: {}", sum);

}

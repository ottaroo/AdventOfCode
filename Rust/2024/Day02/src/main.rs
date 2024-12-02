use std::fs;
use regex::Regex;

fn main() {

    let txt = fs::read_to_string(r"d:\AdventOfCode2023\InputFiles\Day02.txt").unwrap();
    let mut safe_reports = 0;
    let re = Regex::new(r"\d+").unwrap();


    for line in txt.lines() {

        // extract all numbers from line
        let numbers : Vec<i32> = re.find_iter(&line).map(|m| m.as_str().parse::<i32>().unwrap()).collect();
        if is_safe_report(&numbers) {
            safe_reports += 1;
        }

    }

    println!("Safe reports: {}", safe_reports);

}



fn is_safe_report(numbers: &Vec<i32>) -> bool {
    if is_safely_increasing_or_decreasing(numbers) {
        return true;
    }

    let capacity = numbers.len() - 1;
    let mut tmp: Vec<i32> = Vec::with_capacity(capacity);
    tmp.resize(capacity, 0);

    let mut index_to_skip = 0usize;
    for _i in 0..numbers.len() {
        for n in 0..numbers.len() {
            if index_to_skip == n {
                continue;
            }
            let target_index = if n > index_to_skip { n - 1} else { n };
            tmp[target_index] = numbers[n];
        }
        index_to_skip = index_to_skip + 1;
        if is_safely_increasing_or_decreasing(&tmp) {
            return true;
        }
    }

    return false;
}

fn is_safely_increasing_or_decreasing(numbers: &Vec<i32>) -> bool {
    let diff = get_diff(numbers);
    let all_increasing = diff.iter().all(|&x| x > 0 && x <= 3);
    let all_decreasing = diff.iter().all(|&x| x < 0 && x >= -3);
    if all_increasing || all_decreasing {
        return true;
    }

    return false;
}

fn get_diff(numbers: &Vec<i32>) -> Vec<i32> {
    let mut diffs: Vec<i32> = Vec::new();
    for i in 1..numbers.len() {
        diffs.push(numbers[i] - numbers[i - 1]);
    }

    return diffs;
}

-- Bảng "Users"
CREATE TABLE Users (
  user_id INT PRIMARY KEY,
  username VARCHAR(255),
  password VARCHAR(255),
  email VARCHAR(255)
);

-- Bảng "Tests"
CREATE TABLE Tests (
  test_id INT PRIMARY KEY,
  test_name VARCHAR(255),
  test_description VARCHAR(255),
  duration INT
);

-- Bảng "Questions"
CREATE TABLE Questions (
  question_id INT PRIMARY KEY,
  test_id INT,
  question_text VARCHAR(255),
  FOREIGN KEY (test_id) REFERENCES Tests(test_id)
);

-- Bảng "Options"
CREATE TABLE Options (
  option_id INT PRIMARY KEY,
  question_id INT,
  option_text VARCHAR(255),
  is_correct bit,
  FOREIGN KEY (question_id) REFERENCES Questions(question_id)
);

-- Bảng "Results"
CREATE TABLE Results (
  result_id INT PRIMARY KEY,
  user_id INT,
  test_id INT,
  score float,
  start_at datetime,
  submitted_at datetime,
  FOREIGN KEY (user_id) REFERENCES Users(user_id),
  FOREIGN KEY (test_id) REFERENCES Tests(test_id)
);

CREATE TABLE ResultDetail (
  result_detail_id int,
  result_id int,
  question_id Int,
  option_id int,
  FOREIGN KEY (result_id) REFERENCES Results(result_id),
  FOREIGN KEY (question_id) REFERENCES Questions(question_id),
  FOREIGN KEY (option_id) REFERENCES Options(option_id),
  primary key (result_detail_id, result_id, question_id)
);

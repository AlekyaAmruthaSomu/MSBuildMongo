function(){
  print('executing migration');
  var lastRun = db.task.find();
	print('lastRun: ' + lastRun.lastrun);
  var count = db.Sessions.count();
  
  return lastRun;
};

param($installPath, $toolsPath, $package, $project)
	$project.Object.References | Where-Object { $_.Name -eq 'DbExecutor.Contracts' } | ForEach-Object { $_.Remove() }
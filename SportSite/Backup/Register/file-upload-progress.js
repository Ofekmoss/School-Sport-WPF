var FileUploadProgress = {
	UploadStatus: {
		Pending: 1,
		Uploading: 2,
		Finished: 3,
		Aborted: 5
	},
	Items: [],
	Init: function (itemsPlaceholder) {
		this.ItemsPlaceholder = itemsPlaceholder;
	},
	FindItem: function (itemId) {
		var _this = this;
		var matchingItems = _this.Items.filter(function (x) { return x.UniqueId == itemId; });
		return matchingItems.length == 0 ? null : matchingItems[0];
	},
	GetMetaData: function (itemId) {
		var _this = this;
		var uploadItem = _this.FindItem(itemId);
		return uploadItem == null ? null : uploadItem.MetaData;
	},
	UpdateMetaData: function (itemId, key, value) {
		var _this = this;
		var uploadItem = _this.FindItem(itemId);
		if (uploadItem != null) {
			uploadItem.MetaData[key] = value;
		}
	},
	StillInProgress: function () {
		var _this = this;
		for (var i = 0; i < _this.Items.length; i++) {
			var curItem = _this.Items[i];
			if (curItem.Status == _this.UploadStatus.Pending || curItem.Status == _this.UploadStatus.Uploading)
				return true;
		}
		return false;
	},
	GetFirstPending: function () {
		var _this = this;
		for (var i = 0; i < _this.Items.length; i++) {
			var curItem = _this.Items[i];
			if (curItem.Status == _this.UploadStatus.Uploading)
				return null;
			if (curItem.Status == _this.UploadStatus.Pending)
				return curItem;
		}
		return null;
	},
	ApplyCurrentUpload: function () {
		var _this = this;
		var pendingItem = _this.GetFirstPending();
		if (pendingItem != null) {
			pendingItem.Status = _this.UploadStatus.Uploading;
			pendingItem.ItemRow.find(".upload-caption").html("מעלה " + pendingItem.Caption + " אל השרת... ");
			pendingItem.HandleUpload(pendingItem.UniqueId);
		}
	},
	FileUploaded: function (itemId) {
		var _this = this;
		var uploadItem = _this.FindItem(itemId);
		if (uploadItem != null) {
			uploadItem.Status = _this.UploadStatus.Finished;
			uploadItem.ItemRow.find(".success-label").show();
			uploadItem.ItemRow.find(".cancel-upload").hide();
			_this.ApplyCurrentUpload();
			uploadItem.ItemRow.hide(3000, function () {
				if (uploadItem.OnSuccess != null)
					uploadItem.OnSuccess(uploadItem.UniqueId);
				uploadItem.ItemRow.remove();
			});
		}
	},
	AddFile: function (caption, handleUpload, cancelCallback, uploadedCallback, metaData) {
		var _this = this;
		if (typeof metaData == "undefined")
			metaData = {};
		var uniqueId = _this.Items.length + 1;
		var itemRow = $("<div></div>").addClass("row").appendTo(_this.ItemsPlaceholder);
		var itemCol = $("<div></div>").addClass("col-md-12").appendTo(itemRow);
		$("<span></span>").addClass("upload-caption").html(caption + " בתור להעלאה...").appendTo(itemCol);
		var cancelButton = $("<button></button>").attr("type", "button").addClass("btn btn-xs btn-danger cancel-upload").html(" בטל").appendTo(itemCol);
		$("<span></span").addClass("glyphicon glyphicon-remove-sign").prependTo(cancelButton);
		var successLabel = $("<span></span>").addClass("text-success success-label").appendTo(itemCol);
		var successIcon = $("<span></span>").addClass("glyphicon glyphicon-ok").attr("title", "קובץ עלה בהצלחה").appendTo(successLabel);
		successLabel.hide();
		var uploadItem = {
			Caption: caption,
			HandleUpload: handleUpload,
			OnCancel: cancelCallback,
			OnSuccess: uploadedCallback,
			UniqueId: uniqueId,
			Status: _this.UploadStatus.Pending,
			ItemRow: itemRow,
			MetaData: metaData
		};
		_this.Items.push(uploadItem);
		cancelButton.bind("click", function () {
			uploadItem.Status = _this.UploadStatus.Aborted;
			if (uploadItem.OnCancel != null)
				uploadItem.OnCancel(uploadItem.UniqueId);
			$(this).parents(".row").first().remove();
			_this.ApplyCurrentUpload();
		});
		_this.ApplyCurrentUpload();
		return uniqueId;
	}
};